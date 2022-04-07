// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEventsExtension.when_converting_to_committed_events.and_there_is_one_event;

public class and_it_is_the_first_event : given.all_dependencies
{
    Establish context = () =>
    {
        number_of_events_in_aggregate_when_commit_happened = 0;
        protobuf_committed_events = with_committed_events(
            with_committed_event(
                "{\"hello\": \"world\"}",
                0,
                "f35b8b43-4f83-4af9-bfb7-488731c39037",
                1,
                false));
    };

    Because of = () => result = protobuf_committed_events.ToCommittedEvents();

    It should_have_the_same_aggregate_root_id = () => result.AggregateRoot.Value.ShouldEqual(protobuf_committed_events.AggregateRootId.ToGuid());
    It should_have_the_same_event_source_id = () => result.EventSource.Value.ShouldEqual(protobuf_committed_events.EventSourceId);
    It should_have_the_same_number_of_events = () => result.Count.ShouldEqual(protobuf_committed_events.Events.Count);
    It should_have_the_correct_event = () => should_have_correct_event(0, number_of_events_in_aggregate_when_commit_happened);
}