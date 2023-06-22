// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents the basis for a sequence of committed <see cref="Event" >events</see>.
/// </summary>
/// <typeparam name="TEvent">CommittedEvent.</typeparam>
public abstract class CommittedEventSequence<TEvent> : EventSequence<TEvent>
    where TEvent : CommittedEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedEventSequence{T}"/> class.
    /// </summary>
    /// <param name="events">IReadOnlyList of events.</param>
    protected CommittedEventSequence(IReadOnlyList<TEvent> events)
        : base(events)
    {
        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            if (i > 0)
            {
                ThrowIfEventLogSequenceIsOutOfOrder(@event, events[i - 1]);
            }
        }
    }

    static void ThrowIfEventLogSequenceIsOutOfOrder(TEvent @event, TEvent previousEvent)
    {
        if (@event.EventLogSequenceNumber <= previousEvent.EventLogSequenceNumber)
        {
            throw new EventLogSequenceIsOutOfOrder(@event.EventLogSequenceNumber, previousEvent.EventLogSequenceNumber);
        }
    }
}
