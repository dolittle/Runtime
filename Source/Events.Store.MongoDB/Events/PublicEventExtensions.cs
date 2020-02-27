// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="PublicEvent" />.
    /// </summary>
    public static class PublicEventExtensions
    {
        /// <summary>
        /// Gets the <see cref="PublicEventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="PublicEventMetadata" />.</returns>
        public static PublicEventMetadata GetPublicEventMetadata(this CommittedEvent committedEvent) =>
            new PublicEventMetadata(
                committedEvent.EventLogSequenceNumber,
                committedEvent.Occurred,
                committedEvent.EventSource,
                committedEvent.CorrelationId,
                committedEvent.Microservice,
                committedEvent.Tenant,
                committedEvent.Cause.Type,
                committedEvent.Cause.Position,
                committedEvent.Type.Id,
                committedEvent.Type.Generation);

        /// <summary>
        /// Converts a <see cref="PublicEvent" /> to a <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="publicEvent">The <see cref="PublicEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this PublicEvent publicEvent) =>
            new CommittedEvent(
                publicEvent.Metadata.EventLogSequenceNumber,
                publicEvent.Metadata.Occurred,
                publicEvent.Metadata.EventSource,
                publicEvent.Metadata.Correlation,
                publicEvent.Metadata.Microservice,
                publicEvent.Metadata.Tenant,
                new Cause(publicEvent.Metadata.CauseType, publicEvent.Metadata.CausePosition),
                new Artifacts.Artifact(publicEvent.Metadata.TypeId, publicEvent.Metadata.TypeGeneration),
                true,
                publicEvent.Content.ToString());

        /// <summary>
        /// Converts a <see cref="PublicEvent" /> to a <see cref="Runtime.Events.Streams.StreamEvent" />.
        /// </summary>
        /// <param name="publicEvent">The <see cref="PublicEvent" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Streams.StreamEvent" />.</returns>
        public static Runtime.Events.Streams.StreamEvent ToRuntimeStreamEvent(this PublicEvent publicEvent) =>
            new Runtime.Events.Streams.StreamEvent(publicEvent.ToCommittedEvent(), StreamId.PublicEventsId, PartitionId.NotSet);

        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to a <see cref="PublicEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <returns>The converted <see cref="PublicEvent" />.</returns>
        public static PublicEvent ToPublicEvent(this CommittedEvent committedEvent, StreamPosition streamPosition) =>
            new PublicEvent(
                streamPosition,
                committedEvent.GetPublicEventMetadata(),
                BsonDocument.Parse(committedEvent.Content));
    }
}