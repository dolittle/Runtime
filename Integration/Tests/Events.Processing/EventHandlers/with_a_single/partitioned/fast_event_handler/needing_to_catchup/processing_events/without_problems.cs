// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.fast_event_handler.needing_to_catchup.processing_events;

class without_problems : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;

    Establish context = () =>
    {
        commit_events_for_each_event_type(10, "some event source").GetAwaiter().GetResult();
        with_event_handlers((true, number_of_event_types, ScopeId.Default, true));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        stop_event_handlers_after(TimeSpan.FromSeconds(10));
        run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers().GetAwaiter().GetResult();
    };

    It should_have_persisted_stream_definition = () => get_stream_definition_for(event_handler).Success.ShouldBeTrue();
    It should_have_persisted_a_partitioned_stream_definition = () => get_stream_definition_for(event_handler).Result.Partitioned.ShouldBeTrue();
}