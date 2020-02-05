// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEvents
{
    public class when_enumerating : given.two_committed_events
    {
        static CommittedEvents events;
        static CommittedEvent[] enumerated;

        Establish context = () =>
        {
            events = new CommittedEvents(new CommittedEvent[] { first_event, second_event });
        };

        Because of = () =>
        {
            enumerated = events.ToArray();
        };

        It should_enumerate_two_events = () => enumerated.Length.ShouldEqual(2);
        It should_enumerate_the_first_event_first = () => enumerated[0].ShouldEqual(first_event);
        It should_enumerate_the_second_event_second = () => enumerated[1].ShouldEqual(second_event);
    }
}
