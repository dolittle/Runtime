// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedAggregateEvents
{
    public class when_enumerating : given.an_aggregate_instance_and_some_committed_events
    {
        static CommittedAggregateEvents events;
        static CommittedAggregateEvent[] enumerated;

        Establish context = () =>
        {
            events = new CommittedAggregateEvents(event_source_id, aggregate_root_type, 2, new CommittedAggregateEvent[] { first_event, second_event });
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
