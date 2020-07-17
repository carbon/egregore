﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using egregore.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace egregore
{
    internal static class Use
    {
        public static void UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethods.Get)
                    context.Response.Headers.TryAdd("Content-Security-Policy", "default-src 'self'");

                var options = context.RequestServices.GetService<IOptionsSnapshot<WebServerOptions>>();
                if (options != default && options.Value.PublicKey.Length > 0)
                {
                    var keyPin = Convert.ToBase64String(Crypto.Sha256(options.Value.PublicKey));
                    context.Response.Headers.TryAdd("Public-Key-Pins",
                        $"pin-sha256=\"{keyPin}\"; max-age={TimeSpan.FromDays(7).Seconds}; includeSubDomains");
                }

                context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
                context.Response.Headers.TryAdd("Permissions-Policy", "unsized-media 'self'");

                await next();
            });
        }
    }
}