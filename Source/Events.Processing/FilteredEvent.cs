// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a <see cref="CommittedEvent" /> that has been filtered.
    /// </summary>
    public class FilteredEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredEvent"/> class.
        /// </summary>
        /// <param name="version"> The <see cref="CommittedEventVersion" />.</param>
        /// <param name="metadata"> The <see cref="EventMetadata" />.</param>
        /// <param name="event"> The <see cref="IEvent" />.</param>
        /// <param name="partitionId"> The <see cref="PartitionId" />.</param>
        public FilteredEvent(CommittedEventVersion version, EventMetadata metadata, IEvent @event, PartitionId partitionId)
            : base(version, metadata, @event) => PartitionId = partitionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredEvent"/> class.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public FilteredEvent(CommittedEvent @event, PartitionId partitionId)
            : base(@event.Version, @event.Metadata, @event.Event) => PartitionId = partitionId;

        /// <summary>
        /// Gets the <see cref="PartitionId">partition </see> that this <see cref="CommittedEvent" /> belongs to.
        /// </summary>
        public PartitionId PartitionId { get; }
    }
}