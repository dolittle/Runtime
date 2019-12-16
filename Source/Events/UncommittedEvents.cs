// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a stream of events that are uncommitted.
    /// </summary>
    public class UncommittedEvents : IEnumerable<IEvent>
    {
        readonly List<VersionedEvent> _events = new List<VersionedEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvents"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="IEventSource"/>.</param>
        public UncommittedEvents(IEventSource eventSource)
        {
            EventSource = eventSource;
        }

        /// <summary>
        /// Gets the <see cref="IEventSource"/> for the <see cref="UncommittedEvents"/>.
        /// </summary>
        public IEventSource EventSource { get; }

        /// <summary>
        /// Gets the Id of the <see cref="IEventSource"/> that this <see cref="UncommittedEvents"/> relates to.
        /// </summary>
        public EventSourceId EventSourceId => EventSource.EventSourceId;

        /// <summary>
        /// Gets the <see cref="IEvent">events</see> and associated <see cref="EventSourceVersion">version</see>.
        /// </summary>
        public IEnumerable<VersionedEvent> Events => _events.ToArray();

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the Stream.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <summary>
        /// Gets the number of Events in the Stream.
        /// </summary>
        public int Count => _events.Count;

        /// <summary>
        /// Appends an event to the uncommitted event stream, setting the correct EventSourceId and Sequence Number for the event.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/>to be append.</param>
        /// <param name="version">The <see cref="EventSourceVersion">version</see> of the <see cref="IEventSource"/> the <see cref="IEvent"/> is for.</param>
        public void Append(IEvent @event, EventSourceVersion version)
        {
            ThrowIfEventIsNull(@event);
            _events.Add(new VersionedEvent(@event, version));
        }

        /// <inheritdoc/>
        public IEnumerator<IEvent> GetEnumerator()
        {
            return _events.Select(e => e.Event).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        void ThrowIfEventIsNull(IEvent @event)
        {
            // there's no need to check if the event "belongs" to this EventSource.  It's being applied to this one so that makes it belong.
            if (@event == null)
                throw new ArgumentNullException($"Cannot append a null event - {nameof(@event)}");
        }
    }
}