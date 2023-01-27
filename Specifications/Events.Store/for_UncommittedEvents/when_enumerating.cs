// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_UncommittedEvents;

public class when_enumerating : given.events
{
    static UncommittedEvents events;
    static UncommittedEvent[] enumerated;

    Establish context = () =>
    {
        events = new UncommittedEvents(new[] { event_one, event_two, event_three });
    };

    Because of = () =>
    {
        enumerated = events.ToArray();
    };

    It should_enumerate_three_events = () => enumerated.Length.Should().Be(3);
    It should_enumerate_the_first_event_first = () => enumerated[0].Should().Be(event_one);
    It should_enumerate_the_second_event_second = () => enumerated[1].Should().Be(event_two);
    It should_enumerate_the_third_event_third = () => enumerated[2].Should().Be(event_three);
}