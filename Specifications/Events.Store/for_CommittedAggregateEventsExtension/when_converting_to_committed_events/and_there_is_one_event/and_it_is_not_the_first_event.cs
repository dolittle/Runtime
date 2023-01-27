// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEventsExtension.when_converting_to_committed_events.and_there_is_one_event;

public class and_it_is_not_the_first_event : given.all_dependencies
{
    Establish context = () =>
    {
        number_of_events_in_aggregate_when_commit_happened = 5;
        protobuf_committed_events = with_committed_events(
            with_committed_event(
                "{\"testing\": 42}",
                4,
                "d00afd4f-89fc-49fb-8b1c-35a94405d5dd",
                8,
                false));
    };

    Because of = () => result = protobuf_committed_events.ToCommittedEvents();

    It should_have_the_same_aggregate_root_id = () => result.AggregateRoot.Value.Should().Be(protobuf_committed_events.AggregateRootId.ToGuid());
    It should_have_the_same_event_source_id = () => result.EventSource.Value.Should().Be(protobuf_committed_events.EventSourceId);
    It should_have_the_same_number_of_events = () => result.Count.Should().Be(protobuf_committed_events.Events.Count);
    It should_have_the_correct_event = () => should_have_correct_event(0, number_of_events_in_aggregate_when_commit_happened);
}