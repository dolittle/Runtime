// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents a unique identifier for a partition.
    /// </summary>
    public record PartitionId(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Gets the <see cref="PartitionId"/> when no partition is specified.
        /// </summary>
        public static PartitionId None => Guid.Empty.ToString();

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="partition"><see cref="string"/> representation.</param>
        public static implicit operator PartitionId(string partition) => new(partition);
    }
}