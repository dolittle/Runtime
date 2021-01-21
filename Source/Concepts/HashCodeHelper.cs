// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Concepts
{
    /// <summary>
    /// Provides useful methods for dealing with HashCodes.
    /// </summary>
    public static class HashCodeHelper
    {
        /// <summary>
        /// Encapsulates an algorithm for generating a hashcode from a series of parameters.
        /// </summary>
        /// <param name="parameters">Properties to generate the HashCode from.</param>
        /// <returns>The hash code.</returns>
        /// <remarks>
        /// Inspired by:
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode.
        /// </remarks>
        public static int Generate(params object[] parameters)
        {
            unchecked
            {
                return parameters.Where(param => param != null)
                            .Aggregate(17, (current, param) => (current * 29) + param.GetHashCode());
            }
        }

        /// <summary>
        /// Encapsulates an algorithm for generating a hashcode from an enumerable.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> to generate for.</param>
        /// <returns>The hash code.</returns>
        public static int GetHashCode(IEnumerable<object> enumerable)
        {
            unchecked
            {
                int hash = 17;
                foreach (var item in enumerable)
                {
                    hash = (hash * 23) + (item?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }
    }
}