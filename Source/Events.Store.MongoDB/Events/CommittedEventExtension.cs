// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            new(
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
            new(
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
            new(
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
    }
}