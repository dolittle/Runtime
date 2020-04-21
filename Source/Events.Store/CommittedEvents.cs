// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a sequence of events that have been committed to the Event Store.
    /// </summary>
    public class CommittedEvents : CommittedEventSequence<CommittedEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvents"/> class.
        /// </summary>
        /// <param name="events">The <see cref="CommittedEvent">events</see>.</param>
        public CommittedEvents(IReadOnlyList<CommittedEvent> events)
            : base(events)
        {
        }
    }
}
