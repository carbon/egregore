﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// ReSharper disable IdentifierTypo

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace egregore
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Binding")]
    internal static class NativeMethods
    {
        public const string DllName = "libsodium";

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/generating_random_data" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void randombytes_buf(byte* buf, uint size);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/public-key_cryptography/public-key_signatures" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_sign_keypair(byte* pk, byte* sk);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/helpers#hexadecimal-encoding-decoding" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr sodium_bin2hex(byte* hex, int hexMaxlen, byte* bin, int binLen);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/helpers#hexadecimal-encoding-decoding" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr sodium_hex2bin(byte* bin, int binMaxLen, byte* hex, int hexLen);
        
        [DllImport("libsodium", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int sodium_hex2bin(byte* bin, int binMaxlen, byte* hex, int hexLen, string ignore, out int binLen, string hexEnd);
        
        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/advanced/ed25519-curve25519" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_sign_ed25519_sk_to_curve25519(byte* x25519Sk, byte* ed25519Sk);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/public-key_cryptography/public-key_signatures" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_sign_ed25519_sk_to_pk(byte* pk, byte* sk);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/public-key_cryptography/public-key_signatures#detached-mode" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_sign_detached(byte* sig, ref ulong siglen, byte* m, ulong mlen, byte* sk);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/public-key_cryptography/public-key_signatures#detached-mode" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_sign_verify_detached(byte* sig, byte* m, ulong mlen, byte* pk);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/advanced/sha-2_hash_function" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int crypto_hash_sha256(byte* @out, byte* @in, ulong inLen);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/memory_management#zeroing-memory" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void sodium_memzero(void* addr, int len);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/memory_management#locking-memory" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int sodium_mlock(void* addr, int len);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/memory_management#locking-memory" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int sodium_munlock(void* addr, int len);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/usage" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sodium_init();

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/memory_management#guarded-heap-allocations" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* sodium_malloc(int size);

        /// <summary>
        ///     <see href="https://libsodium.gitbook.io/doc/memory_management#guarded-heap-allocations" />
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void sodium_free(void* ptr);
    }

}