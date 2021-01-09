﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace egregore.Simulation
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    public struct Entity : IEquatable<uint>, IEquatable<Entity>, IStructuralEquatable, IStructuralComparable,
        IComparable<Entity>, IComparable
    {
        // ReSharper disable once InconsistentNaming
        public uint Id;

        internal Entity(uint id)
        {
            Id = id;
        }

        public bool Equals(uint other)
        {
            return Id.Equals(other);
        }

        public bool Equals(Entity other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return IdComparer.GetHashCode(this);
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !left.Equals(right);
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            return comparer.Equals(this, other);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode();
        }

        public int CompareTo(object other, IComparer comparer)
        {
            return comparer.Compare(this, other);
        }

        private sealed class IdEqualityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity x, Entity y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Entity obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public int CompareTo(Entity other)
        {
            return Id.CompareTo(other.Id);
        }

        public int CompareTo(object obj)
        {
            return ReferenceEquals(null, obj) ? 1 :
                obj is Entity other ? CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(Entity)}");
        }

        public static bool operator <(Entity left, Entity right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Entity left, Entity right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Entity left, Entity right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Entity left, Entity right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}";
        }

        public static implicit operator Entity(uint id)
        {
            return new Entity(id);
        }

        public static implicit operator uint(Entity entity)
        {
            return entity.Id;
        }

        public static IEqualityComparer<Entity> IdComparer { get; } = new IdEqualityComparer();
    }
}