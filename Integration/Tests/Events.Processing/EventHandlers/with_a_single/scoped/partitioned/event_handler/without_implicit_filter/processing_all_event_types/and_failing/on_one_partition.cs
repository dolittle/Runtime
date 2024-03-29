// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.scoped.partitioned.event_handler.without_implicit_filter.processing_all_event_types.and_failing;


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
        event_handler = setup_event_handler();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, failing_partition.Value), (2, succeeding_partition.Value));
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_with_failure(
        event_handler,
        new failing_partitioned_state(new Dictionary<PartitionId, StreamPosition>{
            {
                failing_partition,
                get_partitioned_events_in_stream(event_handler, failing_partition).First().Position
            }
        }));
}