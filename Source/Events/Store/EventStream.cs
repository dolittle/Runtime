// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An enumerable of <see cref="EventEnvelope" />s that provide a convenient way of dealing with a stream of <see cref="IEvent" />s.
    /// </summary>
    public class EventStream : IEnumerable<EventEnvelope>, IEquatable<EventStream>
    {
        static readonly EnumerableEqualityComparer<EventEnvelope> _comparer = new EnumerableEqualityComparer<EventEnvelope>();
        readonly List<EventEnvelope> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="events"><see cref="IEnumerable{T}"/> of <see cref="EventEnvelope"/>.</param>
        /// <exception type="InvalidEmptyEventStream">Thrown when given an empty or null events.</exception>
        public EventStream(IEnumerable<EventEnvelope> events)
        {
            if (events?.Any() != true)
                throw new InvalidEmptyEventStream(events == null ? $"{nameof(events)} cannot be null" : $"{nameof(events)} cannot be empty");

            _events = events != null ? new List<EventEnvelope>(events) : new List<EventEnvelope>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="EventStream" /> from an <see cref="IEnumerable{T}"/> of <see cref="EventEnvelope" />.
        /// </summary>
        /// <param name="events">The IEnumerable{<see cref="EventEnvelope" />} to initialise the <see cref="EventStream" /> with.</param>
        /// <returns>An instance of <see cref="EventStream" /> initialised with the events.</returns>
        public static EventStream From(IEnumerable<EventEnvelope> events)
        {
            return events as EventStream ?? new EventStream(events);
        }

        /// <summary>
        /// Equates two <see cref="EventStream" /> instances.
        /// </summary>
        /// <param name="first">The first <see cref="EventStream" /> instance.</param>
        /// <param name="second">The second <see cref="EventStream" /> instance.</param>
        /// <returns>true if equal, otherwise false.</returns>
        public static bool Equals(EventStream first, EventStream second)
        {
            return _comparer.Equals(first, second);
        }

        /// <summary>
        /// Creates a new <see cref="EventStream" /> from the current but containing on the specified event types.
        /// </summary>
        /// <param name="artifact"><see cref="ArtifactId" /> of the event type to return.</param>
        /// <returns>A new <see cref="EventStream" /> containing only instances of the specified event type.</returns>
        public EventStream FilteredByEventType(ArtifactId artifact)
        {
            return new EventStream(this.Where(e => e.Metadata.Artifact.Id == artifact));
        }

        /// <summary>
        /// Equates this instance of <see cref="EventStream" /> to another.
        /// </summary>
        /// <param name="other"> The other <see cref="EventStream" /> to equate with.</param>
        /// <returns>true if equal, otherwise false.</returns>
        public bool Equals(EventStream other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as EventStream);
        }

        /// <inheritdoc/>
        public IEnumerator<EventEnvelope> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator() as IEnumerator;
        }
    }
}