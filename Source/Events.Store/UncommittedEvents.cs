// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a sequence of <see cref="UncommittedEvent"/>s that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvents : IReadOnlyList<UncommittedEvent>
    {
        readonly NullFreeList<UncommittedEvent> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvents"/> class.
        /// </summary>
        /// <param name="events">The <see cref="UncommittedEvent">events</see>.</param>
        public UncommittedEvents(IReadOnlyList<UncommittedEvent> events)
        {
            _events = new NullFreeList<UncommittedEvent>(events);
        }

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the uncommitted sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public UncommittedEvent this[int index] => _events[index];

        /// <inheritdoc/>
        public IEnumerator<UncommittedEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();
    }
}