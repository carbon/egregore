﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using egregore.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using WyHash;

namespace egregore.Extensions
{
    internal static class ConditionalHttpExtensions
    {
        private static readonly ulong Seed = BitConverter.ToUInt64(Encoding.UTF8.GetBytes(nameof(ConditionalHttpExtensions)));

        public static bool IsContentStale<TRegion>(this HttpRequest request, string cacheKey,
            ICacheRegion<TRegion> cache, out byte[] stream, out DateTimeOffset? lastModified, out IActionResult result)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-None-Match
            if (!request.IfNoneMatch(cacheKey, cache, out stream, out result))
            {
                lastModified = default;
                return false;
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-Modified-Since
            // "When used in combination with If-None-Match, it is ignored, unless the server doesn't support If-None-Match."
            if (!request.IfModifiedSince(cacheKey, cache, out lastModified, out result))
                return false;

            return true;
        }

        public static bool IfModifiedSince<TRegion>(this HttpRequest request, string cacheKey, ICacheRegion<TRegion> cache, out DateTimeOffset? lastModified, out IActionResult result)
        {
            var headers = request.GetTypedHeaders();
            var ifModifiedSince = headers.IfModifiedSince;
            
            if (cache.TryGetValue($"{cacheKey}:{HeaderNames.LastModified}", out lastModified) && 
                lastModified <= ifModifiedSince)
            {
                result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                return false;
            }

            result = default;
            return true;
        }

        public static bool IfNoneMatch<TRegion>(this HttpRequest request, string cacheKey, ICacheRegion<TRegion> cache, out byte[] stream, out IActionResult result)
        {
            var headers = request.GetTypedHeaders();

            foreach (var etag in headers.IfNoneMatch)
            {
                if (etag.IsWeak)
                {
                    if (!etag.Tag.Equals(cacheKey.WeakETag(false)))
                        continue;
                    result = new StatusCodeResult((int) HttpStatusCode.NotModified);
                    stream = default;
                    return false;
                }

                if (!cache.TryGetValue(cacheKey, out stream) || !etag.Tag.Equals(stream.StrongETag()))
                    continue;
                result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                return false;
            }

            result = default;
            stream = default;
            return true;
        }

        public static void AppendETags(this HttpResponse response, string cacheKey, byte[] stream)
        {
            response.Headers.TryAdd(HeaderNames.ETag, new StringValues(new[] { cacheKey.WeakETag(true), stream.StrongETag() }));
        }

        public static string StrongETag(this byte[] data)
        {
            var value = WyHash64.ComputeHash64(data, Seed);
            return $"\"{value}\"";
        }

        public static string WeakETag(this string key, bool prefix)
        {
            var value = WyHash64.ComputeHash64(Encoding.UTF8.GetBytes(key), Seed);
            return prefix ? $"W/\"{value}\"" : $"\"{value}\"";
        }
    }
}