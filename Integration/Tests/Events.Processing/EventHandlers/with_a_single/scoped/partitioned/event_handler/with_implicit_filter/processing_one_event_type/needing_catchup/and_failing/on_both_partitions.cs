// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.scoped.partitioned.event_handler.with_implicit_filter.processing_one_event_type.needing_catchup.and_failing;

[Ignore("Implicit filter does not work yet with event handlers")]
class on_both_partitions : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static string failure_reason;
    static PartitionId first_failing_partition;
    static PartitionId second_failing_partition;

    Establish context = () =>
    {
        first_failing_partition = "some event source";
        second_failing_partition = "some other event source";
        failure_reason = "some reason";
        commit_events_for_each_event_type((2, first_failing_partition.Value), (2, second_failing_partition.Value));
        fail_for_event_sources(new EventSourceId[]{first_failing_partition.Value, second_failing_partition.Value}, failure_reason);
        event_handler = setup_event_handler();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, first_failing_partition.Value), (2, second_failing_partition.Value));
    };

    It should_the_correct_number_of_events_in_stream = () => expect_number_of_filtered_events(event_handler, scope_events_for_event_types(event_handler_scope, 1).LongCount());
    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_with_failure(
        event_handler,
        new failing_partitioned_state(new Dictionary<PartitionId, StreamPosition>{
            {
                first_failing_partition,
                get_partitioned_events_in_stream(event_handler, first_failing_partition).First().Position
            },
            {
                second_failing_partition,
                get_partitioned_events_in_stream(event_handler, second_failing_partition).First().Position
            }
        }));
}