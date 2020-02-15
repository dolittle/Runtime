// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents a <see cref="Store.CommittedEvent" /> that is a part of a stream.
    /// </summary>
    public class CommittedEventWithPartition : Value<CommittedEventWithPartition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventWithPartition"/> class.
        /// </summary>
        /// <param name="event">The <see cref="Store.CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public CommittedEventWithPartition(Store.CommittedEvent @event, PartitionId partitionId)
        {
            Event = @event;
            PartitionId = partitionId;
        }

        /// <summary>
        /// Gets or sets the <see cref="Store.CommittedEvent" />.
        /// </summary>
        public Store.CommittedEvent Event { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartitionId">partition </see> that this <see cref="Store.CommittedEvent" /> belongs to.
        /// </summary>
        public PartitionId PartitionId { get; set; }
    }
}