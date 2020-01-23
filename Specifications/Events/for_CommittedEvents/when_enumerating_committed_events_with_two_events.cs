// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEvents
{
    public class when_enumerating_committed_events_with_two_events : given.two_committed_events
    {
        static CommittedEvents events;
        static CommittedEvent[] enumeratedEvents;

        Establish context = () =>
        {
            events = new CommittedEvents(new CommittedEvent[] { first_event, second_event });
            enumeratedEvents = new CommittedEvent[2];
        };

        Because of = () =>
        {
            var counter = 0;
            foreach (var @event in events)
            {
                enumeratedEvents[counter] = @event;
                counter++;
            }
        };

        It should_enumerate_the_first_event_first = () => enumeratedEvents[0].ShouldEqual(first_event);
        It should_enumerate_the_second_event_second = () => enumeratedEvents[1].ShouldEqual(second_event);
    }
}
