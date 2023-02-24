// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Processing.EventHandlers.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.event_handler.without_implicit_filter.processing_one_event_type.and_failing;


class after_2_events : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static string failure_reason;
    static PartitionId failing_partition;

    Establish context = () =>
    {
        failing_partition = "some event source";
        failure_reason = "some reason";
        fail_after_processing_number_of_events(2, failure_reason);
        event_handler = setup_event_handler();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, failing_partition.Value));
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler);
    
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_with_failure(
        event_handler,
        new failing_unpartitioned_state(1, failure_reason));
}