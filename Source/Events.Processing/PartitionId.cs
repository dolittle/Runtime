// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a unique identifier for a partition.
    /// </summary>
    public class PartitionId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the not set <see cref="PartitionId" />.
        /// </summary>
        public static PartitionId NotSet => new PartitionId { Value = Guid.Empty };

        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="Guid"/> representation.</param>
        public static implicit operator PartitionId(Guid identifier) => new PartitionId { Value = identifier };

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="PartitionId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="string"/> representation.</param>
        public static implicit operator PartitionId(string identifier) => new PartitionId { Value = Guid.Parse(identifier) };
    }
}