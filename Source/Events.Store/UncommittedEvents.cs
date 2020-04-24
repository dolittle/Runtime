// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a sequence of <see cref="UncommittedEvent"/>s that have not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvents : EventSequence<UncommittedEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvents"/> class.
        /// </summary>
        /// <param name="events">The <see cref="UncommittedEvent">events</see>.</param>
        public UncommittedEvents(IReadOnlyList<UncommittedEvent> events)
            : base(events)
        {
        }
    }
}
