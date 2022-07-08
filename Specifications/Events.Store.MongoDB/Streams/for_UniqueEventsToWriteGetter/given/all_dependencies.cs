// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.given;

public class all_dependencies
{
    protected static UniqueEventsToWriteGetter unique_events_getter;
    protected static List<Events.StreamEvent> stream_events_to_write;
    protected static List<Events.StreamEvent> stored_stream_events;
    protected static List<Events.Event> events_to_write;
    protected static List<Events.Event> stored_events;
    
    protected static IReadOnlyList<Events.StreamEvent> unique_stream_events;
    protected static IReadOnlyList<Events.Event> unique_events;
    protected static EventLogSequenceNumber duplicate_event_log_sequence_number;
    protected static bool result;
    
    Establish context = () =>
    {
        stream_events_to_write = new List<StreamEvent>();
        stored_stream_events = new List<StreamEvent>();
        events_to_write = new List<Events.Event>();
        stored_events = new List<Events.Event>();
        
        unique_events_getter = new UniqueEventsToWriteGetter();
    };

    protected static void get_unique_stream_events()
        => result = unique_events_getter.TryGet(
            stream_events_to_write,
            stored_stream_events,
            out unique_stream_events,
            out duplicate_event_log_sequence_number);
    
    protected static void get_unique_events()
        => result = unique_events_getter.TryGet(
            events_to_write,
            stored_events,
            out unique_events,
            out duplicate_event_log_sequence_number);
}