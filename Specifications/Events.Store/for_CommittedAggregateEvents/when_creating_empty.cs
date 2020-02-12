// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents
{
    public class when_creating_empty : given.events_and_an_artifact
    {
        static CommittedAggregateEvents events;

        Because of = () => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, aggregate_version_before, aggregate_version_before + 0, Array.Empty<CommittedAggregateEvent>());

        It should_not_have_events = () => events.HasEvents.ShouldBeFalse();
        It should_have_a_count_of_zero = () => events.Count.ShouldEqual(0);
    }
}
