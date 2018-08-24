/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a special version of an eventstream
    /// that holds committed <see cref="IEvent">events</see>
    /// </summary>
    public class CommittedEvents : IEnumerable<CommittedEvent>
    {
        List<CommittedEvent> _events = new List<CommittedEvent>();

        /// <summary>
        /// Initializes a new instance of <see cref="CommittedEvents">CommittedEventStream</see>
        /// </summary>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the <see cref="IEventSource"/></param>
        public CommittedEvents(EventSourceId eventSourceId)
        {
            EventSourceId = eventSourceId;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="CommittedEvents">CommittedEvents</see>
        /// </summary>
        /// <param name="eventSourceId">The <see cref="EventSourceId"/> of the <see cref="IEventSource"/></param>
        /// <param name="committedEvents">The <see cref="CommittedEvent">events</see></param>
        public CommittedEvents(EventSourceId eventSourceId, IEnumerable<CommittedEvent> committedEvents)
        {
            EventSourceId = eventSourceId;
            foreach (var committedEvent in committedEvents)
            {
                EnsureEventIsValid(committedEvent);
                _events.Add(committedEvent);
            }
        }

        /// <summary>
        /// Gets the Id of the <see cref="IEventSource"/> that this <see cref="CommittedEvents"/> relates to.
        /// </summary>
        public EventSourceId EventSourceId { get; }

        /// <summary>
        /// Indicates whether there are any events in the Stream.
        /// </summary>
        public bool HasEvents
        {
            get { return Count > 0; }
        }

        /// <summary>
        /// The number of Events in the Stream.
        /// </summary>
        public int Count
        {
            get { return _events.Count; }
        }

        /// <summary>
        /// Get a generic enumerator to iterate over the events
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<CommittedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void EnsureEventIsValid(CommittedEvent committedEvent)
        {
            if (committedEvent.Event == null)
                throw new ArgumentNullException("Cannot append a null event");

            if(committedEvent.Metadata.EventSourceId != this.EventSourceId)
                throw new EventBelongsToOtherEventSource(committedEvent.Metadata?.EventSourceId ?? Guid.NewGuid(),this.EventSourceId);
        }
    }
}