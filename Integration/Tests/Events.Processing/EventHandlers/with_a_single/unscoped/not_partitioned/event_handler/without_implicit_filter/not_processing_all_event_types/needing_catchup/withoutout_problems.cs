// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.event_handler.without_implicit_filter.processing_all_event_types.needing_catchup;


class without_problems : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static EventSourceId event_source;

    Establish context = () =>
    {
        event_source = "some_source";
        commit_events_for_each_event_type((2, "some_source"));
        event_handler = setup_event_handler();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, "some_source"));
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler);
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_without_failure(event_handler);
}