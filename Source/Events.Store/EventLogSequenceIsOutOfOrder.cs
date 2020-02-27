// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when a sequence of events are not in the order they were committed to the Event Store.
    /// </summary>
    public class EventLogSequenceIsOutOfOrder : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogSequenceIsOutOfOrder"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> the Event was committed to.</param>
        /// <param name="expectedSequenceNumber">Expected <see cref="EventLogSequenceNumber"/>.</param>
        public EventLogSequenceIsOutOfOrder(EventLogSequenceNumber sequenceNumber, EventLogSequenceNumber expectedSequenceNumber)
            : base($"Event Log Sequence is out of order. Sequence number '{sequenceNumber}' from event does not match '{expectedSequenceNumber}'")
        {
        }
    }
}
