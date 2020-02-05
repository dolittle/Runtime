// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.Collections;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a sequence of <see cref="IEvent"/>s that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvents : IReadOnlyList<IEvent>
    {
        readonly NullFreeList<IEvent> _events = new NullFreeList<IEvent>();

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the uncommitted sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public IEvent this[int index] => _events[index];

        /// <summary>
        /// Appends an event to the uncommitted sequence.
        /// </summary>
        /// <param name="event"><see cref="IEvent"/> to append.</param>
        public void Append(IEvent @event)
        {
            ThrowIfEventIsNull(@event);
            _events.Add(@event);
        }

        /// <inheritdoc/>
        public IEnumerator<IEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(IEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }
    }
}