﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace egregore.Cryptography
{
    public interface IKeyFileService : IDisposable
    {
        FileStream GetKeyFileStream();
        unsafe byte* GetSecretKeyPointer(IKeyCapture capture, [CallerMemberName] string callerMemberName = null);
    }
}