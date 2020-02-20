// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="ReceivedEvent" />.
    /// </summary>
    public static class ReceivedEventExtensions
    {
        /// <summary>
        /// Gets the <see cref="ReceivedEventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <param name="producerTenant">The <see cref="TenantId" /> that produced the event.</param>
        /// <returns>The converted <see cref="ReceivedEventMetadata" />.</returns>
        public static ReceivedEventMetadata GetReceivedEventMetadata(this CommittedEvent committedEvent, TenantId producerTenant) =>
            new ReceivedEventMetadata(
                committedEvent.Occurred,
                committedEvent.EventSource,
                committedEvent.CorrelationId,
                committedEvent.Microservice,
                committedEvent.Tenant,
                producerTenant,
                committedEvent.EventLogVersion,
                committedEvent.Type.Id,
                committedEvent.Type.Generation);

        /// <summary>
        /// Converts a <see cref="ReceivedEvent" /> to a <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="ReceivedEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this ReceivedEvent @event) =>
            new CommittedEvent(
                @event.StreamPosition,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.Metadata.Correlation,
                @event.Metadata.Microservice,
                @event.Metadata.ReceiverTenant,
                new Cause(CauseType.Event, 0),
                new Artifacts.Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                false,
                @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="ReceivedEvent" /> to a <see cref="Streams.StreamEvent" />.
        /// </summary>
        /// <param name="publicEvent">The <see cref="ReceivedEvent" />.</param>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <returns>The converted <see cref="Streams.StreamEvent" />.</returns>
        public static Streams.StreamEvent ToRuntimeStreamEvent(this ReceivedEvent publicEvent, Microservice microservice) =>
            new Streams.StreamEvent(publicEvent.ToCommittedEvent(), microservice.Value, PartitionId.NotSet);

        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to a <see cref="ReceivedEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="producerTenant">The <see cref="TenantId" />.</param>
        /// <returns>The converted <see cref="ReceivedEvent" />.</returns>
        public static ReceivedEvent ToReceivedEvent(this CommittedEvent committedEvent, StreamPosition streamPosition, TenantId producerTenant) =>
            new ReceivedEvent(
                streamPosition,
                committedEvent.GetReceivedEventMetadata(producerTenant),
                BsonDocument.Parse(committedEvent.Content));
    }
}