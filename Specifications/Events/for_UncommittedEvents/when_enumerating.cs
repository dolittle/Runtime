// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedEvents
{
    public class when_enumerating : given.Events
    {
        static UncommittedEvents events;
        static IEvent[] enumerated;

        Establish context = () =>
        {
            events = new UncommittedEvents();
            events.Append(event_one);
            events.Append(event_two);
        };

        Because of = () =>
        {
            enumerated = events.ToArray();
        };

        It should_enumerate_two_events = () => enumerated.Length.ShouldEqual(2);
        It should_enumerate_the_first_event_first = () => enumerated[0].ShouldEqual(event_one);
        It should_enumerate_the_second_event_second = () => enumerated[0].ShouldEqual(event_one);
    }
}
