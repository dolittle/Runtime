// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when an event is being used with an Event Source with a different <see cref="EventSourceId"/> than it was applied to.
    /// </summary>
    public class EventWasAppliedToOtherEventSource : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventWasAppliedToOtherEventSource"/> class.
        /// </summary>
        /// <param name="eventEventSource">The <see cref="EventSourceId"/> the Event was applied to.</param>
        /// <param name="eventSource"><see cref="EventSourceId"/> of the Event Source.</param>
        public EventWasAppliedToOtherEventSource(EventSourceId eventEventSource, EventSourceId eventSource)
            : base($"Event Source '{eventEventSource}' from event does not match with '{eventSource}'.")
        {
        }
    }
}
