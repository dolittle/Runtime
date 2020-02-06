// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedAggregateEvents
{
    public class when_creating : given.an_aggregate_and_two_events
    {
        static UncommittedAggregateEvents events;

        Because of = () => events = new UncommittedAggregateEvents(event_source_id, aggregate_root_type, aggregate_root_version);

        It should_not_have_events = () => events.HasEvents.ShouldBeFalse();
        It should_have_a_count_of_zero = () => events.Count.ShouldEqual(0);
    }
}
