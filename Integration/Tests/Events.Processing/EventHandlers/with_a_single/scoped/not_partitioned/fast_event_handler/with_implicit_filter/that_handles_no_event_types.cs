// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.scoped.not_partitioned.fast_event_handler.with_implicit_filter;

class that_handles_no_event_types : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static Exception exception;
    Establish context = () =>
    {
        with_event_handlers_filtering_number_of_event_types(0);
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        commit_events_after_starting_event_handler((2, "some_source"));
    };

    It should_have_persisted_correct_stream = () => expect_stream_definition(event_handler, 0);
    It should_the_correct_number_of_events_in_stream = () => expect_number_of_filtered_events(event_handler, committed_events_for_event_types(0).LongCount());
    It should_have_the_correct_stream_processor_states = () => expect_stream_processor_state_without_failure(event_handler, 0!);

}