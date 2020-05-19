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
    /// </summary>
    [BsonDiscriminator(RootClass = true, Required = true)]
    [BsonKnownTypes(typeof(StreamProcessorState), typeof(Partitioned.PartitionedStreamProcessorState))]
    [BsonIgnoreExtraElements]
    public abstract class AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStreamProcessorState"/> class.
        /// </summary>
        /// <param name="eventProcessorId">The <see cref="EventProcessor" />.</param>
        /// <param name="sourceStreamId">The <see cref="SourceStream" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="partitioned">Whether it is partitioned.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        protected AbstractStreamProcessorState(
            Guid eventProcessorId,
            Guid sourceStreamId,
            ulong position,
            bool partitioned,
            DateTime lastSuccessfullyProcessed)
        {
            EventProcessor = eventProcessorId;
            SourceStream = sourceStreamId;
            Position = position;
            Partitioned = partitioned;
            LastSuccessfullyProcessed = lastSuccessfullyProcessed;
        }

        /// <summary>
        /// Gets or sets the event processor id.
        /// </summary>
        public Guid EventProcessor { get; set; }

        /// <summary>
        /// Gets or sets the source stream id.
        /// </summary>
        public Guid SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stream processor is processing a partitioned stream.
        /// </summary>
        public bool Partitioned { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the StreamProcessor has processed the stream with Kind of UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastSuccessfullyProcessed { get; set; }
    }
}
