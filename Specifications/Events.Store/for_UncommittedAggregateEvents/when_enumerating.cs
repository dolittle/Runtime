// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_UncommittedAggregateEvents
{
    public class when_enumerating : given.events_and_an_aggregate
    {
        static UncommittedAggregateEvents events;
        static UncommittedEvent[] enumerated;

        Establish context = () =>
        {
            events = new UncommittedAggregateEvents(event_source_id, aggregate_artifact, aggregate_version, new[] { event_one, event_two, event_three });
        };

        Because of = () =>
        {
            enumerated = events.ToArray();
        };

        It should_enumerate_three_events = () => enumerated.Length.ShouldEqual(3);
        It should_enumerate_the_first_event_first = () => enumerated[0].ShouldEqual(event_one);
        It should_enumerate_the_second_event_second = () => enumerated[1].ShouldEqual(event_two);
        It should_enumerate_the_third_event_third = () => enumerated[2].ShouldEqual(event_three);
    }
}
