// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when a sequence of <see cref="IEvent"/>s are not in the order they were committed to the Event Store.
    /// </summary>s
    public class EventLogVersionIsOutOfOrder : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogVersionIsOutOfOrder"/> class.
        /// </summary>
        /// <param name="eventVersion">The <see cref="EventLogVersion"/> the Event was committed to.</param>
        /// <param name="expectedVersion">Expected <see cref="EventLogVersion"/>.</param>
        public EventLogVersionIsOutOfOrder(EventLogVersion eventVersion, EventLogVersion expectedVersion)
            : base($"Event Log Root version is out of order. Version '{eventVersion}' from event does not match '{expectedVersion}'")
        {
        }
    }
}
