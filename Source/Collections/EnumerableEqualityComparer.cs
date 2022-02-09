// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Collections;

/// <summary>
/// Compares to enumerables and returns true if the same elements are in both collections in the same order.
/// </summary>
/// <typeparam name="T">Type that gets compared.</typeparam>
public class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
{
    /// <summary>
    /// Generates a hashcode for the enumberable, utilising the hashcode of each element.
    /// </summary>
    /// <param name="enumerable">The enumerable to generte the hashcode for.</param>
    /// <returns>The hashcode value.</returns>
    /// <remarks>
    /// Inspired by:
    /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode.
    /// </remarks>
    public int GetHashCode(IEnumerable<T> enumerable)
    {
        if (enumerable != null)
        {
            unchecked
            {
                var hash = 17;
                foreach (var item in enumerable)
                {
                    hash = (hash * 23) + ((item != null) ? item.GetHashCode() : 0);
                }

                return hash;
            }
        }

        return 0;
    }

    /// <summary>
    /// Equates two <see cref="IEnumerable{T}">enumerables</see>.
    /// </summary>
    /// <param name="left">Left <see cref="IEnumerable{T}"/>.</param>
    /// <param name="right">Right <see cref="IEnumerable{T}"/>.</param>
    /// <returns>True if the exact same elements, in the same order, are in both enumerables.</returns>
    public bool Equals(IEnumerable<T> left, IEnumerable<T> right)
    {
        if (object.ReferenceEquals(left, right) || (left == null && right == null))
        {
            return true;
        }

        if (left == null || right == null)
        {
            return false;
        }

        var firstArray = left.ToArray();
        var secondArray = right.ToArray();

        if (firstArray.Length != secondArray.Length)
        {
            return false;
        }

        for (var i = 0; i < firstArray.Length; i++)
        {
            if (!object.Equals(firstArray[i], secondArray[i]))
            {
                return false;
            }
        }

        return true;
    }
}