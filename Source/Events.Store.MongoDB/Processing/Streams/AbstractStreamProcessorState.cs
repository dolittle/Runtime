// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the base state of an <see cref="AbstractScopedStreamProcessor" />.
    /// It has a programmatically assigned <see cref="StreamProcessorStateDiscriminatorConvention"/> which takes care of
    /// serializing <see cref="StreamProcessorState"/> and <see cref="Partitioned.PartitionedStreamProcessorState"/> to
    /// this collection.
    /// </summary>
    [BsonKnownTypes(typeof(StreamProcessorState), typeof(Partitioned.PartitionedStreamProcessorState))]
    [BsonIgnoreExtraElements]
    public abstract class AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStreamProcessorState"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="SourceStreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        protected AbstractStreamProcessorState(
            Guid scopeId,
            Guid eventProcessorId,
            Guid sourceStreamId,
            ulong position,
            DateTimeOffset lastSuccessfullyProcessed)
        {
            ScopeId = scopeId;
            EventProcessorId = eventProcessorId;
            SourceStreamId = sourceStreamId;
            Position = position;
            LastSuccessfullyProcessed = lastSuccessfullyProcessed;
        }

        /// <summary>
        /// Gets or sets the scope id.
        /// </summary>
        public Guid ScopeId { get; set; }

        /// <summary>
        /// Gets or sets the event processor id.
        /// </summary>
        public Guid EventProcessorId { get; set; }

        /// <summary>
        /// Gets or sets the source stream id.
        /// </summary>
        public Guid SourceStreamId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Position { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the StreamProcessor has processed the stream.
        /// </summary>
        [BsonRepresentation(BsonType.Document)]
        public DateTimeOffset LastSuccessfullyProcessed { get; set; }
    }
}
