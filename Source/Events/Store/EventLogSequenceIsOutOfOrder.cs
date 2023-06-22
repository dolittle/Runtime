// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when a event log sequence numbers in a sequence of events are out of order, meaning that an event has a lower event log sequence number than the previous event.
/// </summary>
public class EventLogSequenceIsOutOfOrder : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventLogSequenceIsOutOfOrder"/> class.
    /// </summary>
    /// <param name="sequenceNumber">The <see cref="EventLogSequenceNumber"/> the Event was committed to.</param>
    /// <param name="expectedSequenceNumber">Expected <see cref="EventLogSequenceNumber"/>.</param>
    public EventLogSequenceIsOutOfOrder(EventLogSequenceNumber sequenceNumber, EventLogSequenceNumber expectedSequenceNumber)
        : base($"Event Log Sequence is out of order because Event Log Sequence Number '{sequenceNumber}' is not greater than '{expectedSequenceNumber}'.")
    {
    }
}