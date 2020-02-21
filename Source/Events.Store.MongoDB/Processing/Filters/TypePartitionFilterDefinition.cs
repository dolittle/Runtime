// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinition
    {
        /// <summary>
        /// Gets or sets the <see cref="TypePartitionFilterDefinitionId" />.
        /// </summary>
        [BsonId]
        public TypePartitionFilterDefinitionId Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Artifacts.ArtifactId"> types</see> that this definition filters on.
        /// </summary>
        public IEnumerable<Guid> Types { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this type filter is partitioned.
        /// </summary>
        public bool Partitioned { get; set; }

        /// <summary>
        /// Creates a store representation of a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <returns>The store representation <see cref="TypePartitionFilterDefinition" />.</returns>
        public static TypePartitionFilterDefinition FromDefinition(TypeFilterWithEventSourcePartitionDefinition definition, StreamId targetStream, StreamId sourceStream) =>
            new TypePartitionFilterDefinition
            {
                Id = new TypePartitionFilterDefinitionId { SourceStream = sourceStream, TargetStream = targetStream },
                Partitioned = definition.Partitioned,
                Types = definition.Types.Select(_ => _.Value)
            };

        /// <summary>
        /// Whether this <see cref="TypePartitionFilterDefinition" /> has the same definition as another <see cref="TypePartitionFilterDefinition" />.
        /// </summary>
        /// <param name="other">The other <see cref="TypePartitionFilterDefinition" />.</param>
        /// <returns>Whether they have the same defintion.</returns>
        public bool HasSameDefinitionAs(TypePartitionFilterDefinition other) =>
            Partitioned == other.Partitioned
            && Types.Count() == other.Types.Count()
            && Types.All(other.Types.Contains);
    }
}