// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;
using mongoDB = Dolittle.Runtime.Events.Store.MongoDB.Events;
using runtime = Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventConverter" />.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        /// <inheritdoc/>
        public runtime.Streams.StreamEvent ToRuntimeStreamEvent(mongoDB.Event @event) =>
            new runtime.Streams.StreamEvent(@event.ToCommittedEvent(), @event.EventLogSequenceNumber, StreamId.EventLog, Guid.Empty, false);

        /// <inheritdoc/>
        public runtime.Streams.StreamEvent ToRuntimeStreamEvent(StreamEvent @event, StreamId stream, bool partitioned) =>
            new runtime.Streams.StreamEvent(@event.ToCommittedEvent(), @event.StreamPosition, stream, @event.Partition, partitioned);

        /// <inheritdoc/>
        public mongoDB.Event ToScopedEventLogEvent(CommittedEvent @event, EventLogSequenceNumber eventLogSequenceNumber) =>
            new mongoDB.Event(
                eventLogSequenceNumber,
                @event.ExecutionContext.ToStoreRepresentation(),
                @event.GetEventMetadata(true),
                new AggregateMetadata(),
                BsonDocument.Parse(@event.Content));

        /// <inheritdoc/>
        public StreamEvent ToScopedStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            ToStoreStreamEvent(committedEvent, streamPosition, partition, true);

        /// <inheritdoc/>
        public mongoDB.StreamEvent ToStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            ToStoreStreamEvent(committedEvent, streamPosition, partition, false);

        mongoDB.StreamEvent ToStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition, bool fromEventHorizon) =>
            new MongoDB.Events.StreamEvent(
                streamPosition,
                partition,
                committedEvent.ExecutionContext.ToStoreRepresentation(),
                committedEvent.GetStreamEventMetadata(fromEventHorizon),
                committedEvent.GetAggregateMetadata(),
                BsonDocument.Parse(committedEvent.Content));
    }
}