// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.unpartitioned.fast_event_handler.with_implicit_filter.unprocessing_all_event_types.and_failing;

[Ignore("Implicit filter does not work yet with event handlers")]
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
        fail_for_event_sources(new EventSourceId[]{failing_partition.Value}, failure_reason);
        with_event_handlers_filtering_number_of_event_types((false, number_of_event_types, ScopeId.Default, true, true));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(
            (2, failing_partition.Value, ScopeId.Default),
            (2, succeeding_partition.Value, ScopeId.Default)).GetAwaiter().GetResult();
    };

    It should_the_correct_number_of_events_in_stream = () => expect_number_of_filtered_events(event_handler, committed_events_for_event_types(number_of_event_types).LongCount());
    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler, partitioned: false, public_stream: false, max_handled_event_types: number_of_event_types);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state(
        event_handler,
        implicit_filter: false,
        partitioned: false,
        num_events_to_handle: committed_events.Count,
        failing_partitioned_state: new failing_partitioned_state(new Dictionary<PartitionId, StreamPosition>{
            {
                failing_partition,
                get_partitioned_events_in_stream(event_handler, failing_partition).First().Position
            }
        }),
        failing_unpartitioned_state: null);
}