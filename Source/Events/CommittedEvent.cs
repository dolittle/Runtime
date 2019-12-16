// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represent an instance of an Event that has been committed to the Event Store.
    /// </summary>
    public class CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvent"/> class.
        /// </summary>
        /// <param name="version">The committed event version.</param>
        /// <param name="metadata">Metadata describing the event.</param>
        /// <param name="event">The <see cref="IEvent">event</see> instance.</param>
        public CommittedEvent(CommittedEventVersion version, EventMetadata metadata, IEvent @event)
        {
            Version = version;
            Metadata = metadata;
            Event = @event;
        }

        /// <summary>
        /// Gets the Version of this Event, including the CommitSequenceNumber, the Event Source Commit Version and the Event Sequence Number.
        /// </summary>
        public CommittedEventVersion Version { get; }

        /// <summary>
        /// Gets the metadata describing the event.
        /// </summary>
        public EventMetadata Metadata { get; }

        /// <summary>
        /// Gets the Id of the Event.
        /// </summary>
        public EventId Id => Metadata.Id;

        /// <summary>
        /// Gets the instance of the <see cref="IEvent">event</see> that was committed.
        /// </summary>
        public IEvent Event { get; }
    }
}