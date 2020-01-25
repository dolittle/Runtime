// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedAggregateEvents
{
    public class when_creating : given.an_aggregate_instance_and_some_committed_events
    {
        static CommittedAggregateEvents events;

        Because of = () =>
        {
            events = new CommittedAggregateEvents(event_source_id, aggregate_root_type, aggregate_root_version, Array.Empty<CommittedAggregateEvent>());
        };

        It should_have_events = () => events.HasEvents.ShouldBeFalse();
        It should_have_a_count_of_one = () => events.Count.ShouldEqual(0);
    }
}
