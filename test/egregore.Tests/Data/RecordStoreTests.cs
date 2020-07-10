﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using egregore.Data;
using Xunit;
using Record = egregore.Data.Record;

namespace egregore.Tests.Data
{
    public class RecordStoreTests
    {
        [Fact]
        public async Task Empty_store_has_zero_length()
        {
            using var fixture = new RecordStoreFixture();
            var count = await fixture.Store.GetLengthAsync();
            Assert.Equal(0UL, count);
        }

        [Fact]
        public async Task Can_append_record()
        {
            using var fixture = new RecordStoreFixture();

            var record = new Record {Type = "Customer"};
            record.Columns.Add(new RecordColumn(0, "Order", "int", "123"));
            
            var next = await fixture.Store.AddRecordAsync(record);
            Assert.Equal(1UL, next);

            var count = await fixture.Store.GetLengthAsync();
            Assert.Equal(1UL, count);
        }
        
        private class CustomerV1
        {
            public int Order { get; set; }
        }
    }
}