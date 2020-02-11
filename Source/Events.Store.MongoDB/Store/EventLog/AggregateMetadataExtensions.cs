// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.EventLog
{
    /// <summary>
    /// Extension methods for <see cref="AggregateMetadata" />.
    /// </summary>
    public static class AggregateMetadataExtensions
    {
        /// <summary>
        /// Gets the <see cref="AggregateMetadata" /> from a <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <returns>The <see cref="AggregateMetadata"/>.</returns>
        public static AggregateMetadata GetAggregateMetadata(this CommittedEvent committedEvent) =>
            committedEvent is CommittedAggregateEvent aggregateEvent ? new AggregateMetadata
                {
                    WasAppliedByAggregate = true,
                    EventSourceId = aggregateEvent.EventSource,
                    Version = aggregateEvent.AggregateRootVersion,
                    TypeId = aggregateEvent.AggregateRoot.Id,
                    TypeGeneration = aggregateEvent.AggregateRoot.Generation
                }
                : new AggregateMetadata();
    }
}