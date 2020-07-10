﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using egregore.Configuration;
using egregore.IO;
using egregore.Ontology;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace egregore
{
    public sealed class WebServer
    {
        public WebServer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void Run(string eggPath, IKeyCapture capture, params string[] args)
        {
            PrintMasthead();

            var builder = CreateHostBuilder(eggPath, capture, args);
            var host = builder.Build();
            host.Run();
        }

        private static void PrintMasthead()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@"                                        
                       @@@@             
                  @   @@@@@@            
                  @@@   @@    @@@@      
                   .@@@@@@@@@@@@@@@     
        @@@@@@@@@@@@  @@@@@@@@@         
       @@@@@@@@@@@@@@      %@@@@&       
        .@/       @@@   .@@@@@@@@@@@    
           @@  @@@@@   @@@@@@     %@@   
   @       @@@@@@@    @@@@@@       @@@  
   @@      @@@@@@@@@@@@@@@         @@@  
   @@              @@@  @         @@@@  
    @@@            @@@@ @@@@@@@@@@@@@   
    .@@@@@      @@@@@@/  @@@@@@@@@@@    
      @@@@@@@@@@@@@@@      @@@@@@       
         @@@@@@@@@@                     
");
            Console.ResetColor();
        }

        internal static unsafe IHostBuilder CreateHostBuilder(string eggPath, IKeyCapture capture, params string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Activity.DefaultIdFormat = ActivityIdFormat.W3C;

                    webBuilder.ConfigureKestrel((context, options) => { options.AddServerHeader = false; });
                    webBuilder.ConfigureLogging((context, loggingBuilder) => { });
                    webBuilder.ConfigureAppConfiguration((context, configBuilder) =>
                    {
                        configBuilder.AddEnvironmentVariables();
                    });
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        services.AddCors(o => o.AddDefaultPolicy(b =>
                        {
                            b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                            b.DisallowCredentials();
                        }));

                        var keyFileService = new ServerKeyFileService();
                        services.AddSingleton<IKeyFileService>(keyFileService);

                        capture ??= new ServerConsoleKeyCapture();
                        services.AddSingleton(capture);

                        if (capture is IPersistedKeyCapture persisted)
                            services.AddSingleton(persisted);

                        var publicKey = new byte[Crypto.PublicKeyBytes];

                        try
                        {
                            capture.Reset();
                            var sk = Crypto.LoadSecretKeyPointerFromFileStream(keyFileService.GetKeyFilePath(),
                                keyFileService.GetKeyFileStream(), capture);
                            Crypto.PublicKeyFromSecretKey(sk, publicKey);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                            Environment.Exit(-1);
                        }

                        services.Configure<WebServerOptions>(o =>
                        {
                            o.PublicKey = publicKey;
                            o.EggPath = eggPath;
                        });
                    });
                    webBuilder.Configure((context, appBuilder) => { appBuilder.UseCors(); });
                    webBuilder.UseStartup<WebServer>();
                });
            return builder;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHostedService<WebServerStartup>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // manually instanced singletons are not cleaned up by DI on application exit
            app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
                app.ApplicationServices.GetRequiredService<IPersistedKeyCapture>().Dispose());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethods.Get)
                    context.Response.Headers.TryAdd("Content-Security-Policy", "default-src 'self'");

                var options = context.RequestServices.GetService<IOptionsSnapshot<WebServerOptions>>();
                if (options != default && options.Value.PublicKey.Length > 0)
                {
                    var keyPin = Convert.ToBase64String(Crypto.Sha256(options.Value.PublicKey));
                    context.Response.Headers.TryAdd("Public-Key-Pins", $"pin-sha256=\"{keyPin}\"; max-age={TimeSpan.FromDays(7).Seconds}; includeSubDomains");
                }

                context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
                context.Response.Headers.TryAdd("Feature-Policy", "unsized-media 'self'");

                await next();
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}