﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Threading;
using System.Threading.Tasks;
using egregore.Media;

namespace egregore.Models
{
    public interface IMediaEventHandler
    {
        Task OnMediaAddedAsync(IMediaStore store, MediaEntry entry, CancellationToken cancellationToken = default);
    }
}