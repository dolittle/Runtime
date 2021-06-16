// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the basis for a sequence of <see cref="Event" >events</see>.
    /// </summary>
    /// <typeparam name="TEvent">IReadOnlyList of CommittedEvent or UncommittedEvent.</typeparam>
    public abstract class EventSequence<TEvent> : IReadOnlyList<TEvent>
        where TEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSequence{T}"/> class.
        /// </summary>
        /// <param name="events">IReadOnlyList of events.</param>
        protected EventSequence(IReadOnlyList<TEvent> events)
        {
            events.ForEach(ThrowIfEventIsNull);
            Events = new NullFreeList<TEvent>(events);
        }

        /// <inheritdoc/>
        public int Count => Events.Count;

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the committed sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <summary>
        /// Gets the events.
        /// </summary>
        protected NullFreeList<TEvent> Events { get; }

        /// <inheritdoc/>
        public TEvent this[int index] => Events[index];

        /// <inheritdoc/>
        public IEnumerator<TEvent> GetEnumerator() => Events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => Events.GetEnumerator();

        void ThrowIfEventIsNull(TEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}
