﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace egregore
{
    public interface ILogEntryHashProvider
    {
        byte[] ComputeHashBytes(LogEntry entry);
        byte[] ComputeHashBytes(ILogSerialized data);
        byte[] ComputeHashRootBytes(LogEntry entry);
    }
}