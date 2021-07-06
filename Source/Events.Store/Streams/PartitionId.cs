// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents a unique identifier for a partition.
    /// </summary>
    public record PartitionId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Gets the <see cref="PartitionId"/> when no partition is specified.
        /// </summary>
        public static PartitionId None => Guid.Empty;

        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="Guid"/> representation.</param>
        public static implicit operator PartitionId(Guid identifier) => new(identifier);

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="string"/> representation.</param>
        public static implicit operator PartitionId(string identifier) => Guid.Parse(identifier);

    }
}