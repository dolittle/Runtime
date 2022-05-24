// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.fast_event_handler.processing_one_event_type;

class without_problems : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;

    Establish context = () =>
    {
        with_event_handlers((true, 1, ScopeId.Default, true, false));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(
            (2, "some_source", ScopeId.Default)).GetAwaiter().GetResult();
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(
        event_handler,
        partitioned: true,
        public_stream: false,
        max_handled_event_types: 1);

    It should_the_correct_number_of_events_in_stream = () => expect_number_of_filtered_events(event_handler, committed_events_for_event_types(1).LongCount());

    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state(
        event_handler,
        implicit_filter: false,
        partitioned: true,
        num_events_to_handle: 2,
        failing_partitioned_state: null,
        failing_unpartitioned_state: null);
}