// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a <see cref="CommittedEvent" /> that is a part of a stream.
    /// </summary>
    public class CommittedEventWithPartition : Value<CommittedEventWithPartition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventWithPartition"/> class.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public CommittedEventWithPartition(CommittedEvent @event, PartitionId partitionId)
        {
            Event = @event;
            PartitionId = partitionId;
        }

        /// <summary>
        /// Gets or sets the <see cref="CommittedEvent" />.
        /// </summary>
        public CommittedEvent Event { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartitionId">partition </see> that this <see cref="CommittedEvent" /> belongs to.
        /// </summary>
        public PartitionId PartitionId { get; set; }
    }
}