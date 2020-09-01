﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace egregore.Simulation
{
    internal static class Combinations
    {
        static Combinations()
        {
            for(var i = 0; i < 8; i++)
                GetIndices(i + 1);
        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IList<T> values, int k = 0)
        {
            if (k < 0 || values == null)
                yield break;

            if (k == 0 || values.Count < k)
                k = values.Count;

            var indices = GetIndices(k);
            while (true)
            {
                yield return YieldCombination(values, indices, k);

                var n = k - 1;
                while (n >= 0 && indices[n] >= values.Count - k + n)
                    n--;
                if (n < 0)
                    yield break;

                for (var j = indices[n] + 1; n < k; n++, j++)
                    indices[n] = j;
            }
        }

        private static IEnumerable<T> YieldCombination<T>(IList<T> values, IReadOnlyList<int> indices, int k)
        {
            for (var i = 0; i < k; i++)
                yield return values[indices[i]];
        }

        private static int[] GetIndices(int k)
        {
            return InitializeIndices(k);
        }

        private static int[] InitializeIndices(int k)
        {
            var indices = new int[k];
            for (var i = 0; i < k; i++) indices[i] = i;
            return indices;
        }
    }
}