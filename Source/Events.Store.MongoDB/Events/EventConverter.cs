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
        public mongoDB.Event ToScopedEventLogEvent(CommittedExternalEvent @event) =>
            new mongoDB.Event(
                @event.EventLogSequenceNumber,
                @event.ExecutionContext.ToStoreRepresentation(),
                @event.GetEventMetadata(),
                new AggregateMetadata(),
                new EventHorizonMetadata(@event.ExternalEventLogSequenceNumber, @event.Received, @event.Consent),
                BsonDocument.Parse(@event.Content));

        /// <inheritdoc/>
        public mongoDB.StreamEvent ToScopedStoreStreamEvent(CommittedExternalEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            new mongoDB.StreamEvent(
                streamPosition,
                partition,
                committedEvent.ExecutionContext.ToStoreRepresentation(),
                committedEvent.GetStreamEventMetadata(),
                committedEvent.GetAggregateMetadata(),
                new EventHorizonMetadata(committedEvent.EventLogSequenceNumber, committedEvent.Received, committedEvent.Consent),
                BsonDocument.Parse(committedEvent.Content));

        /// <inheritdoc/>
        public mongoDB.StreamEvent ToStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            new mongoDB.StreamEvent(
                streamPosition,
                partition,
                committedEvent.ExecutionContext.ToStoreRepresentation(),
                committedEvent.GetStreamEventMetadata(),
                committedEvent.GetAggregateMetadata(),
                new EventHorizonMetadata(),
                BsonDocument.Parse(committedEvent.Content));
    }
}