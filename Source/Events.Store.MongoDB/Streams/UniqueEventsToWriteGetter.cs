// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents an implementation of <see cref="IGetUniqueEventsToWrite"/>.
/// </summary>
public class UniqueEventsToWriteGetter : IGetUniqueEventsToWrite
{
    /// <inheritdoc />
    public bool TryGet<TEvent>(IReadOnlyList<TEvent> eventsToWrite, IReadOnlyList<TEvent> storedEvents, out IReadOnlyList<TEvent>? uniqueEvents, out EventLogSequenceNumber? duplicateEventLogSequenceNumber)
        where TEvent : IStoredEvent
    {
        if (AnyListIsEmpty(eventsToWrite, storedEvents, out uniqueEvents))
        {
            duplicateEventLogSequenceNumber = default;
            return true;
        }

        var storedIndex = 0;
        var toCheckIndex = 0;
        
        var uniqueEventsList = new List<TEvent>();
        while (storedIndex < storedEvents.Count && toCheckIndex < eventsToWrite.Count)
        {
            var storedEvent = storedEvents[storedIndex];
            var eventToCheck = eventsToWrite[toCheckIndex];
            var storedEventSequenceNumber = storedEvent.GetEventLogSequenceNumber();
            var eventToCheckSequenceNumber = eventToCheck.GetEventLogSequenceNumber();
            if (storedEventSequenceNumber.Value > eventToCheckSequenceNumber.Value)
            {
                uniqueEventsList.Add(eventToCheck);
                toCheckIndex++;
            }
            else if (eventToCheckSequenceNumber.Value > storedEventSequenceNumber.Value)
            {
                storedIndex++;
            }
            else if (eventToCheck.IsTheSameAs(storedEvent))
            {
                storedIndex++;
                toCheckIndex++;
            }
            else
            {
                duplicateEventLogSequenceNumber = storedEventSequenceNumber;
                uniqueEvents = default;
                return false;
            }
        }

        while (toCheckIndex < eventsToWrite.Count)
        {
            uniqueEventsList.Add(eventsToWrite[toCheckIndex++]);
        }

        duplicateEventLogSequenceNumber = default;
        uniqueEvents = uniqueEventsList;
        return true;
    }


    static bool AnyListIsEmpty<TEvent>(IReadOnlyList<TEvent> eventsToWrite, IReadOnlyList<TEvent> storedEvents, out IReadOnlyList<TEvent>? uniqueEvents)
        where TEvent : IStoredEvent
    {
        if (eventsToWrite.Count == 0) 
        {
            uniqueEvents = Enumerable.Empty<TEvent>().ToList();
            return true;
        }

        if (storedEvents.Count == 0)
        {
            uniqueEvents = eventsToWrite;
            return true;
        }
        uniqueEvents = default;
        return false;
    }
}
