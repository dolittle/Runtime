// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Exception that gets throw when the computed hash to use for constructing an <see cref="EventSourceId"/> from a <see cref="ProjectionKey"/> is not the correct bit length.
    /// </summary>
    public class ComputedHashIsNotOfCorrectLength : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedHashIsNotOfCorrectLength"/> class.
        /// </summary>
        /// <param name="computedHashLength">The length of the computed hash in bits.</param>
        /// <param name="expectedHashLength">The expected length of the hash in bits.</param>
        public ComputedHashIsNotOfCorrectLength(int computedHashLength, int expectedHashLength)
             : base($"The calculated hash was {computedHashLength} bits long, expected {expectedHashLength} bits.")
        {
        }
    }
}