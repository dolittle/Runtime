// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="CommittedEvent" />.
    /// </summary>
    public static class CommittedEventExtension
    {
        /// <summary>
        /// Gets the <see cref="EventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="EventMetadata" />.</returns>
        public static EventMetadata GetEventMetadata(this CommittedEvent committedEvent) =>
            new EventMetadata(
                committedEvent.Occurred.UtcDateTime,
                committedEvent.EventSource,
                committedEvent.Type.Id,
                committedEvent.Type.Generation,
                committedEvent.Public);

        /// <summary>
        /// Gets the <see cref="StreamEventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="StreamEventMetadata" />.</returns>
        public static StreamEventMetadata GetStreamEventMetadata(this CommittedEvent committedEvent) =>
            new StreamEventMetadata(
                committedEvent.EventLogSequenceNumber,
                committedEvent.Occurred.UtcDateTime,
                committedEvent.EventSource,
                committedEvent.Type.Id,
                committedEvent.Type.Generation,
                committedEvent.Public);

        /// <summary>
        /// Gets the <see cref="EventHorizonMetadata"/> from the <see cref="CommittedExternalEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedExternalEvent"/>.</param>
        /// <returns>The converted <see cref="EventHorizonMetadata" />.</returns>
        public static EventHorizonMetadata GetEventHorizonMetadata(this CommittedExternalEvent committedEvent) =>
            new EventHorizonMetadata(
                committedEvent.ExternalEventLogSequenceNumber,
                committedEvent.Received.UtcDateTime,
                committedEvent.Consent);

        /// <summary>
        /// Gets the <see cref="EventHorizonMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="EventHorizonMetadata" />.</returns>
        public static EventHorizonMetadata GetEventHorizonMetadata(this CommittedEvent committedEvent) =>
            committedEvent is CommittedExternalEvent externalEvent ?
                externalEvent.GetEventHorizonMetadata()
                : new EventHorizonMetadata();

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this Event @event) =>
            @event.Aggregate.WasAppliedByAggregate ?
                @event.ToCommittedAggregateEvent()
                : @event.EventHorizon.FromEventHorizon ?
                    new CommittedExternalEvent(
                        @event.EventLogSequenceNumber,
                        @event.Metadata.Occurred,
                        @event.Metadata.EventSource,
                        @event.ExecutionContext.ToExecutionContext(),
                        new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                        @event.Metadata.Public,
                        @event.Content.ToString(),
                        @event.EventHorizon.ExternalEventLogSequenceNumber,
                        @event.EventHorizon.Received,
                        @event.EventHorizon.Consent)
                    : new CommittedEvent(
                      @event.EventLogSequenceNumber,
                      @event.Metadata.Occurred,
                      @event.Metadata.EventSource,
                      @event.ExecutionContext.ToExecutionContext(),
                      new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                      @event.Metadata.Public,
                      @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="StreamEvent" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this StreamEvent @event) =>
            @event.Aggregate.WasAppliedByAggregate ?
                @event.ToCommittedAggregateEvent()
                : @event.EventHorizon.FromEventHorizon ?
                    new CommittedExternalEvent(
                        @event.Metadata.EventLogSequenceNumber,
                        @event.Metadata.Occurred,
                        @event.Metadata.EventSource,
                        @event.ExecutionContext.ToExecutionContext(),
                        new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                        @event.Metadata.Public,
                        @event.Content.ToString(),
                        @event.EventHorizon.ExternalEventLogSequenceNumber,
                        @event.EventHorizon.Received,
                        @event.EventHorizon.Consent)
                    : new CommittedEvent(
                        @event.Metadata.EventLogSequenceNumber,
                        @event.Metadata.Occurred,
                        @event.Metadata.EventSource,
                        @event.ExecutionContext.ToExecutionContext(),
                        new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                        @event.Metadata.Public,
                        @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedAggregateEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedAggregateEvent" />.</returns>
        public static CommittedAggregateEvent ToCommittedAggregateEvent(this Event @event) =>
            new CommittedAggregateEvent(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedAggregateEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedAggregateEvent" />.</returns>
        public static CommittedAggregateEvent ToCommittedAggregateEvent(this StreamEvent @event) =>
            new CommittedAggregateEvent(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.Metadata.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                @event.Content.ToString());
    }
}