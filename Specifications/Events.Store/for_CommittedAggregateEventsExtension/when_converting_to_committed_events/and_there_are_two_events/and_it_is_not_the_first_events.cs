// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.for_CommittedAggregateEventsExtension.given;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEventsExtension.when_converting_to_committed_events.and_there_are_two_events;

public class and_it_is_not_the_first_events : all_dependencies
{
    Establish context = () =>
    {
        number_of_events_in_aggregate_when_commit_happened = 5;
        protobuf_committed_events = with_committed_events(
            with_committed_event(
                "{\"hello\": \"world\"}",
                0,
                "f35b8b43-4f83-4af9-bfb7-488731c39037",
                1,
                false),
            with_committed_event(
                "{\"hello\": \"42\"}",
                1,
                "f35b8b43-4f83-4af9-bfb7-488731c39037",
                3,
                true));
    };

    Because of = () => result = protobuf_committed_events.ToCommittedEvents();

    It should_have_the_same_aggregate_root_id = () => result.AggregateRoot.Value.Should().Be(protobuf_committed_events.AggregateRootId.ToGuid());
    It should_have_the_same_event_source_id = () => result.EventSource.Value.Should().Be(protobuf_committed_events.EventSourceId);
    It should_have_the_same_number_of_events = () => result.Count.Should().Be(protobuf_committed_events.Events.Count);
    It should_have_the_correct_first_event = () => should_have_correct_event(0, number_of_events_in_aggregate_when_commit_happened);
    It should_have_the_correct_second_event = () => should_have_correct_event(1, number_of_events_in_aggregate_when_commit_happened + 1);
}