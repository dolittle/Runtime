// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing.Streams;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the state of an <see cref="AbstractStreamProcessor" />.
    /// </summary>
    public class StreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="SourceStreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="failingPartitions">The states of the failing partitions.</param>
        public StreamProcessorState(Guid scopeId, Guid eventProcessorId, Guid sourceStreamId, ulong position, IDictionary<string, FailingPartitionState> failingPartitions)
        {
            ScopeId = scopeId;
            EventProcessorId = eventProcessorId;
            SourceStreamId = sourceStreamId;
            Position = position;
            FailingPartitions = failingPartitions;
        }

        /// <summary>
        /// Gets or sets the  MongoDB _id. This is used so that the class would have a valid '_id' field in mongo.
        /// The classes 'true' id is comporomised from the combinaton of ScopeId, EventProcessorId and SourceStreamId.
        /// </summary>
        public ObjectId Id { get; set; }

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
        /// Gets or sets the failing partitions.
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public IDictionary<string, FailingPartitionState> FailingPartitions { get; set; }

        /// <summary>
        /// Creates a new, initial, <see cref="Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState" /> from a <see cref="Runtime.Events.Processing.EventProcessorId" />.
        /// </summary>
        /// <param name="id">The <see cref="StreamProcessorId" />.</param>
        /// <returns>The new initial <see cref="StreamProcessorState" />.</returns>
        public static StreamProcessorState NewFromId(Runtime.Events.Processing.Streams.StreamProcessorId id) =>
            new StreamProcessorState(
                id.ScopeId,
                id.EventProcessorId,
                id.SourceStreamId,
                0,
                new Dictionary<string, FailingPartitionState>());
    }
}
