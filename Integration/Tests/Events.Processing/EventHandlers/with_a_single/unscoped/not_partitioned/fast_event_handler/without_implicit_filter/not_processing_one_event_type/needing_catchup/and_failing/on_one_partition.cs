// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.fast_event_handler.without_implicit_filter.processing_one_event_type.needing_catchup.and_failing;


class on_one_partition : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static string failure_reason;
    static PartitionId failing_partition;
    static PartitionId succeeding_partition;

    Establish context = () =>
    {
        failing_partition = "some event source";
        succeeding_partition = "some other event source";
        failure_reason = "some reason";
        commit_events_for_each_event_type((2, failing_partition.Value), (2, succeeding_partition.Value));
        fail_for_event_sources(new EventSourceId[]{failing_partition.Value}, failure_reason);
        event_handler = setup_event_handler();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, failing_partition.Value), (2, succeeding_partition.Value));
    };

    It should_the_correct_number_of_events_in_stream = () => expect_number_of_filtered_events(event_handler, committed_events_for_event_types(1).LongCount());
    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_with_failure(
        event_handler,
        new failing_unpartitioned_state(get_partitioned_events_in_stream(event_handler, failing_partition).First().Position));
}