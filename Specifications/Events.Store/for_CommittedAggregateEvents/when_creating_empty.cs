// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEvents;

public class when_creating_empty : given.events_and_an_artifact
{
    static CommittedAggregateEvents events;

    Because of = () => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, 0, Array.Empty<CommittedAggregateEvent>());

    It should_not_have_events = () => events.HasEvents.ShouldBeFalse();
    It should_have_a_count_of_zero = () => events.Count.ShouldEqual(0);
    It should_have_the_correct_aggregate_root_version = () => events.AggregateRootVersion.Value.ShouldEqual(0ul);
}