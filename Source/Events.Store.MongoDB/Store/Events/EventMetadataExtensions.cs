// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="EventMetadata" />.
    /// </summary>
    public static class EventMetadataExtensions
    {
        /// <summary>
        /// Gets the <see cref="EventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="EventMetadata" />.</returns>
        public static EventMetadata GetEventMetadata(this CommittedEvent committedEvent) =>
            new EventMetadata(
                committedEvent.Occurred,
                committedEvent.EventSource,
                committedEvent.CorrelationId,
                committedEvent.Microservice,
                committedEvent.Tenant,
                committedEvent.Cause.Type,
                committedEvent.Cause.Position,
                committedEvent.Type.Id,
                committedEvent.Type.Generation);
    }
}