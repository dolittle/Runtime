// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.fast_event_handler;

class that_handles_no_event_types : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static Exception exception;
    Establish context = () =>
    {
        commit_events_for_each_event_type(new (int number_of_events, EventSourceId event_source, ScopeId scope_id)[]
        {
            (2, "some_source", ScopeId.Default)
        }).GetAwaiter().GetResult();
        with_event_handlers((true, 0, ScopeId.Default, true));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        exception = Catch.Exception(() => run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(
            (2, "some_source", ScopeId.Default)).GetAwaiter().GetResult());
    };

    It should_fail_when_starting = () => exception.ShouldNotBeNull();

}