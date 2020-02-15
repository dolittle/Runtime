// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents a <see cref="Store.CommittedEvent" /> that is a part of a stream.
    /// </summary>
    public class StreamEvent : Value<StreamEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamEvent"/> class.
        /// </summary>
        /// <param name="event">The <see cref="Store.CommittedEvent" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        public StreamEvent(Store.CommittedEvent @event, StreamId stream, PartitionId partition)
        {
            Event = @event;
            Stream = stream;
            Partition = partition;
        }

        /// <summary>
        /// Gets the <see cref="Store.CommittedEvent" />.
        /// </summary>
        public Store.CommittedEvent Event { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" /> stream that this event is a part of.
        /// </summary>
        public StreamId Stream { get; }

        /// <summary>
        /// Gets the <see cref="PartitionId">partition </see> that this event belongs to.
        /// </summary>
        public PartitionId Partition { get; }
    }
}