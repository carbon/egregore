﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using egregore.Extensions;

namespace egregore.Network
{
    public sealed class SocketServer : IDisposable
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ManualResetEvent _signal = new ManualResetEvent(false);
        private readonly TextWriter _out;
        private Task _task;

        public SocketServer(TextWriter @out = default)
        {
            _out = @out;
        }
        
        public void Start(string hostNameOrIpAddress, int port, CancellationToken cancellationToken = default)
        {
            var ipHostInfo = Dns.GetHostEntry(hostNameOrIpAddress);
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, port);
            _task = Task.Run(() => { AcceptConnection(ipAddress, localEndPoint, cancellationToken); }, cancellationToken);
        }

        private void AcceptConnection(IPAddress address, EndPoint endpoint, CancellationToken cancellationToken)
        {
            var listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(endpoint);
                listener.Listen(100);

                while (true)
                {
                    _signal.Reset();
                    _out?.WriteInfoLine("Waiting for a connection...");
                    var ar = listener.BeginAccept(AcceptCallback, listener);
                    while (!_signal.WaitOne(10) && !ar.IsCompleted)
                        cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                _out?.WriteInfoLine("Server thread was cancelled.");
            }
            catch (Exception e)
            {
                _out?.WriteErrorLine(e.ToString());
            }
            finally
            {
                _out?.WriteInfo("Closing server connection... ");
                listener.Close();
                listener.Dispose();
                _out?.WriteInfoLine("done.");
            }
        }
        
        public void AcceptCallback(IAsyncResult ar)
        {
            _signal.Set();
            var listener = (Socket) ar.AsyncState;
            var handler = listener.EndAccept(ar);
            var socketState = new SocketState {Handler = handler};
            handler.BeginReceive(socketState.buffer, 0, SocketState.BufferSize, 0, ReadCallback, socketState);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            var socketState = (SocketState) ar.AsyncState;
            var handler = socketState.Handler;

            var bytesRead = handler.EndReceive(ar);
            if (bytesRead <= 0)
                return;

            socketState.sb.Append(_encoding.GetString(socketState.buffer, 0, bytesRead));
            var content = socketState.sb.ToString();
            if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
            {
                _out?.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                SendMessage(handler, content);
            }
            else
            {
                handler.BeginReceive(socketState.buffer, 0, SocketState.BufferSize, 0, ReadCallback, socketState);
            }
        }

        private void SendMessage(Socket handler, string message)
        {
            var byteData = _encoding.GetBytes(message);
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }
        
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket) ar.AsyncState;
                var bytesSent = handler.EndSend(ar);
                _out?.WriteInfoLine("Sent {0} bytes to client.", bytesSent);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                _out?.WriteErrorLine(e.ToString());
            }
        }

        public void Dispose()
        {
            while (!_task.IsCanceled && !_task.IsCompleted && !_task.IsFaulted)
                _signal.WaitOne(10);
            _signal?.Dispose();
            _task?.Dispose();
        }
    }
}