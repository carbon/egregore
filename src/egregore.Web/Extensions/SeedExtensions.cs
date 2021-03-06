﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Text;
using egregore.Web.Models;

namespace egregore.Web.Extensions
{
    internal static class SeedExtensions
    {
        public static ulong GetSeed<T>(this ICacheRegion<T> cache)
        {
            if (!cache.TryGetValue(nameof(GetSeed), out ulong seed))
                cache.Set(nameof(GetSeed), seed = BitConverter.ToUInt64(Encoding.UTF8.GetBytes(typeof(T).Name)));

            return seed;
        }
    }
}