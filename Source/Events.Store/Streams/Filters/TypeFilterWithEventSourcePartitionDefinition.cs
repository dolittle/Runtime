// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams.Filters
{
    /// <summary>
    /// Represents a <see cref="FilterDefinition" /> for type filter with event source partition.
    /// </summary>
    public class TypeFilterWithEventSourcePartitionDefinition : FilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="types"><see cref="IEnumerable{T}"/> of <see cref="ArtifactId"/>.</param>
        /// <param name="partitioned">Whether or not to partition by <see cref="EventSourceId"/>.</param>
        public TypeFilterWithEventSourcePartitionDefinition(StreamId sourceStream, StreamId targetStream, IEnumerable<ArtifactId> types, bool partitioned)
            : base(sourceStream, targetStream, partitioned)
        {
            Types = types;
        }

        /// <summary>
        /// Gets the artifacts included by the filter.
        /// </summary>
        public IEnumerable<ArtifactId> Types { get; }
    }
}