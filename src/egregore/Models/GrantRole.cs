﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using egregore.Data;

namespace egregore.Models
{
    public sealed class GrantRole : Privilege
    {
        public GrantRole(string role, byte[] grantor, byte[] grantee, byte[] signature = null) : base(
            Constants.Commands.GrantRole, signature)
        {
            Value = role;
            Authority = grantor;
            Subject = grantee;
        }

        // ReSharper disable once UnusedMember.Global (needed for deserialization)
        public GrantRole(LogDeserializeContext context) : base(context)
        {
        }
    }
}