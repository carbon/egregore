﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace egregore.Web.Models
{
    public sealed class DynamicActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public CancellationTokenSource TokenSource { get; private set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }

        public void OnChanged()
        {
            TokenSource?.Cancel();
        }
    }
}