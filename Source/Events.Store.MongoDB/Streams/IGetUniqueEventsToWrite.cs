// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

public interface IGetUniqueEventsToWrite
{
    bool TryGet<TEvent>(IReadOnlyList<TEvent> eventsToWrite, IReadOnlyList<TEvent> storedEvents, out IReadOnlyList<TEvent>? uniqueEvents, out EventLogSequenceNumber? duplicateEventLogSequenceNumber)
    where TEvent : IStoredEvent;
}
