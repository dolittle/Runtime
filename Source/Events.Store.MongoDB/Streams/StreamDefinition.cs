// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents the store representation of <see cref="IStreamDefinition" />.
    /// </summary>
    public class StreamDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinition"/> class.
        /// </summary>
        /// <param name="streamId">The Stream Id.</param>
        /// <param name="filterDefinition">The <see cref="AbstractFilterDefinition" />.</param>
        /// <param name="partitioned">Whether or not the stream definition is partitioned.</param>
        /// <param name="public">Whether or not the stream definition is public.</param>
        public StreamDefinition(Guid streamId, AbstractFilterDefinition filterDefinition, bool partitioned, bool @public)
        {
            StreamId = streamId;
            Filter = filterDefinition;
            Partitioned = partitioned;
            Public = @public;
        }

        /// <summary>
        /// Gets or sets the Stream Id.
        /// </summary>
        [BsonId]
        public Guid StreamId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this defines a partitioned stream.
        /// </summary>
        public bool Partitioned { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this defines a public stream.
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AbstractFilterDefinition" />.
        /// </summary>
        public AbstractFilterDefinition Filter { get; set; }

        /// <summary>
        /// Converts the stored Stream Definition to the Runtime <see cref="IStreamDefinition" /> representation.
        /// </summary>
        /// <returns><see cref="IStreamDefinition" />.</returns>
        public IStreamDefinition AsRuntimeRepresentation() =>
            new Store.Streams.StreamDefinition(Filter.AsRuntimeRepresentation(
                StreamId,
                Partitioned,
                Public));
    }
}
