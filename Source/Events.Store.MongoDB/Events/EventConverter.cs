// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using mongoDB = Dolittle.Runtime.Events.Store.MongoDB.Events;
using runtime = Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventConverter" />.
    /// </summary>
    public class EventConverter : IEventConverter
    {
        readonly IEventContentConverter _contentConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverter"/> class.
        /// </summary>
        /// <param name="contentConverter">The <see cref="IEventContentConverter"/>.</param>
        public EventConverter(IEventContentConverter contentConverter)
        {
            _contentConverter = contentConverter;
        }

        /// <inheritdoc/>
        public mongoDB.Event ToEventLogEvent(CommittedExternalEvent committedEvent) =>
            new(
                committedEvent.EventLogSequenceNumber,
                committedEvent.ExecutionContext.ToStoreRepresentation(),
                committedEvent.GetEventMetadata(),
                new AggregateMetadata(),
                committedEvent.GetEventHorizonMetadata(),
                _contentConverter.ToBson(committedEvent.Content));

        /// <inheritdoc/>
        public mongoDB.StreamEvent ToStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            new(
                streamPosition,
                partition,
                committedEvent.ExecutionContext.ToStoreRepresentation(),
                committedEvent.GetStreamEventMetadata(),
                committedEvent.GetAggregateMetadata(),
                committedEvent.GetEventHorizonMetadata(),
                _contentConverter.ToBson(committedEvent.Content));

        /// <inheritdoc/>
        public runtime.Streams.StreamEvent ToRuntimeStreamEvent(mongoDB.Event @event) =>
            new(
                ToRuntimeCommittedEvent(@event),
                @event.EventLogSequenceNumber,
                StreamId.EventLog,
                PartitionId.None,
                false);

        /// <inheritdoc/>
        public runtime.Streams.StreamEvent ToRuntimeStreamEvent(mongoDB.StreamEvent @event, StreamId stream, bool partitioned) =>
            new(
                ToRuntimeCommittedEvent(@event),
                @event.StreamPosition,
                stream,
                @event.Partition,
                partitioned);

        runtime.CommittedEvent ToRuntimeCommittedEvent(mongoDB.Event @event) =>
            @event.Aggregate.WasAppliedByAggregate
                ? ToRuntimeCommittedAggregateEvent(@event)
                : @event.EventHorizon.FromEventHorizon
                    ? ToRuntimeCommittedExternalEvent(@event)
                    : new runtime.CommittedEvent(
                        @event.EventLogSequenceNumber,
                        @event.Metadata.Occurred,
                        @event.Metadata.EventSource,
                        @event.ExecutionContext.ToExecutionContext(),
                        new Artifact(
                            @event.Metadata.TypeId,
                            @event.Metadata.TypeGeneration),
                        @event.Metadata.Public,
                        _contentConverter.ToJson(@event.Content));

        runtime.CommittedAggregateEvent ToRuntimeCommittedAggregateEvent(mongoDB.Event @event) =>
            new(
                new Artifact(
                    @event.Aggregate.TypeId,
                    @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(
                    @event.Metadata.TypeId,
                    @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                _contentConverter.ToJson(@event.Content));

        runtime.CommittedExternalEvent ToRuntimeCommittedExternalEvent(mongoDB.Event @event) =>
            new(
                @event.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(
                    @event.Metadata.TypeId,
                    @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                _contentConverter.ToJson(@event.Content),
                @event.EventHorizon.ExternalEventLogSequenceNumber,
                @event.EventHorizon.Received,
                @event.EventHorizon.Consent);

        runtime.CommittedEvent ToRuntimeCommittedEvent(mongoDB.StreamEvent @event) =>
            @event.Aggregate.WasAppliedByAggregate
                ? ToRuntimeCommittedAggregateEvent(@event)
                : @event.EventHorizon.FromEventHorizon
                    ? ToRuntimeCommittedExternalEvent(@event)
                    : new runtime.CommittedEvent(
                        @event.Metadata.EventLogSequenceNumber,
                        @event.Metadata.Occurred,
                        @event.Metadata.EventSource,
                        @event.ExecutionContext.ToExecutionContext(),
                        new Artifact(
                            @event.Metadata.TypeId,
                            @event.Metadata.TypeGeneration),
                        @event.Metadata.Public,
                        _contentConverter.ToJson(@event.Content));

        runtime.CommittedAggregateEvent ToRuntimeCommittedAggregateEvent(mongoDB.StreamEvent @event) =>
            new(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.Metadata.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(
                    @event.Metadata.TypeId,
                    @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                _contentConverter.ToJson(@event.Content));

        runtime.CommittedExternalEvent ToRuntimeCommittedExternalEvent(mongoDB.StreamEvent @event) =>
            new(
                @event.Metadata.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                _contentConverter.ToJson(@event.Content),
                @event.EventHorizon.ExternalEventLogSequenceNumber,
                @event.EventHorizon.Received,
                @event.EventHorizon.Consent);
    }
}
