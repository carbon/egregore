﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using egregore.Data;

namespace egregore
{
    public interface IRecordStore
    {
        string DataFile { get; }
        Task<ulong> GetLengthAsync();

        Task<ulong> GetCountAsync(string type);

        Task<ulong> AddRecordAsync(Record record, byte[] secretKey = null);

        Task<Record> GetByIdAsync(Guid uuid);
        Task<IEnumerable<Record>> GetByColumnValueAsync(string type, string name, string value);

        void Destroy(bool destroySequence);
    }
}