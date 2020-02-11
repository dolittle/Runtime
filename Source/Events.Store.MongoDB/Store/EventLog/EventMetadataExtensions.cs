// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.EventLog
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
            new EventMetadata
            {
                CausePosition = committedEvent.Cause.Position,
                CauseType = committedEvent.Cause.Type,
                Correlation = committedEvent.CorrelationId,
                Microservice = committedEvent.Microservice,
                Occurred = committedEvent.Occurred,
                Tenant = committedEvent.Tenant,
                TypeGeneration = committedEvent.Type.Generation,
                TypeId = committedEvent.Type.Id
            };
    }
}