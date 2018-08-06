using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An enumerable of <see cref="EventEnvelope" />s that provide a convenient way of dealing with a stream of <see cref="IEvent" />s
    /// </summary>
    public class EventStream : IEnumerable<EventEnvelope>, IEquatable<EventStream>
    {
        List<EventEnvelope> _events;
        private static readonly EnumerableComparer<EventEnvelope> _comparer = new EnumerableComparer<EventEnvelope>();

        /// <summary>
        /// Instantiates a new instance of <see cref="EventStream" /> initialized with events.
        /// Empty or null events 
        /// </summary>
        /// <param name="events"></param>
        public EventStream(IEnumerable<EventEnvelope> events)
        {
            if(events == null || !events.Any())
                throw new InvalidEmptyEventStream(events == null ? $"{nameof(events)} cannot be null" : $"{nameof(events)} cannot be empty");

            _events = events != null ? new List<EventEnvelope>(events) : new List<EventEnvelope>();
        }

        /// <summary>
        /// Equates two <see cref="EventStream" /> instances
        /// </summary>
        /// <param name="first">The first <see cref="EventStream" /> instance</param>
        /// <param name="second">The second <see cref="EventStream" /> instance</param>
        /// <returns>true if equal, otherwise false</returns>
        public static bool Equals(EventStream first, EventStream second)
        {
            return _comparer.Equals(first,second);
        }

        /// <summary>
        /// Equates this instance of <see cref="EventStream" /> to another
        /// </summary>
        /// <param name="other"> The other <see cref="EventStream" /> to equate with</param>
        /// <returns>true if equal, false otherwise</returns>
        public bool Equals(EventStream other)
        {
            return Equals(this,other);
        }

        /// <summary>
        /// Gets an enumerator for iterating over the <see cref="EventEnvelope" />s
        /// </summary>
        /// <returns></returns>
        public IEnumerator<EventEnvelope> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        /// <summary>
        /// Calculates the hashcode for this 
        /// </summary>
        /// <returns>the hashcode</returns>
        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator() as IEnumerator;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EventStream" /> from an IEnumerable{<see cref="EventEnvelope" />} 
        /// </summary>
        /// <param name="events">The IEnumerable{<see cref="EventEnvelope" />} to initialise the <see cref="EventStream" /> with</param>
        /// <returns>An instance of <see cref="EventStream" /> initialised with the events</returns>
        public static EventStream From(IEnumerable<EventEnvelope> events)
        {
            return events as EventStream ?? new EventStream(events);
        }
    }
}


