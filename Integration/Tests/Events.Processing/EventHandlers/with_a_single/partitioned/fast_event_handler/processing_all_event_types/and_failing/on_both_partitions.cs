// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.fast_event_handler.processing_all_event_types.and_failing;

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
        fail_for_partitions(new []{first_failing_partition, second_failing_partition}, failure_reason);
        with_event_handlers((true, number_of_event_types, ScopeId.Default, true));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        stop_event_handlers_after(TimeSpan.FromSeconds(2));
        run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(
            (2, first_failing_partition.Value, ScopeId.Default),
            (2, second_failing_partition.Value, ScopeId.Default)).GetAwaiter().GetResult();
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler, partitioned: true, public_stream: false, max_handled_event_types: number_of_event_types);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state(
        event_handler,
        implicit_filter: false,
        partitioned: true,
        num_events_to_handle: committed_events.Count,
        failing_partitioned_state: new failing_partitioned_state(new Dictionary<PartitionId, StreamPosition>{
            {
                first_failing_partition,
                get_partitioned_events_in_stream(event_handler, first_failing_partition).First().Position
            },
            {
                second_failing_partition,
                get_partitioned_events_in_stream(event_handler, second_failing_partition).First().Position
            }
        }),
        failing_unpartitioned_state: null);
}