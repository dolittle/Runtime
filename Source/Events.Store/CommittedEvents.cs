// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections;
using System.Collections.Generic;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a sequence of events that have been committed to the Event Store.
    /// </summary>
    public class CommittedEvents : IReadOnlyList<CommittedEvent>
    {
        readonly NullFreeList<CommittedEvent> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvents"/> class.
        /// </summary>
        /// <param name="events">The <see cref="CommittedEvent">events</see>.</param>
        public CommittedEvents(IReadOnlyList<CommittedEvent> events)
        {
            for (var i = 0; i < events.Count; i++)
            {
                var @event = events[i];
                ThrowIfEventIsNull(@event);
                if (i > 0) ThrowIfEventLogVersionIsOutOfOrder(@event, events[i - 1]);
            }

            _events = new NullFreeList<CommittedEvent>(events);
        }

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the committed sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public CommittedEvent this[int index] => _events[index];

        /// <inheritdoc/>
        public IEnumerator<CommittedEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(CommittedEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }

        void ThrowIfEventLogVersionIsOutOfOrder(CommittedEvent @event, CommittedEvent previousEvent)
        {
            if (@event.EventLogVersion != previousEvent.EventLogVersion + 1) throw new EventLogVersionIsOutOfOrder(@event.EventLogVersion, previousEvent.EventLogVersion + 1);
        }
    }
}