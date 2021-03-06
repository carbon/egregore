﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace egregore.Data.Tests.Fixtures
{
    public class SequenceFixture : IDisposable
    {
        public SequenceFixture()
        {
            Sequence = new Sequence(string.Empty, $"{Guid.NewGuid()}");
        }

        public Sequence Sequence { get; }

        public void Dispose()
        {
            Sequence.Destroy();
        }
    }
}