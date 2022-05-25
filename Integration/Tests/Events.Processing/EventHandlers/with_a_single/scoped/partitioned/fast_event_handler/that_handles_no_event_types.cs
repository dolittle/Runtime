// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.scoped.partitioned.fast_event_handler;

class that_handles_no_event_types : given.single_tenant_and_event_handlers
{
    static IEventHandler event_handler;
    static Exception exception;
    Establish context = () =>
    {
        commit_events_for_each_event_type(new (int number_of_events, EventSourceId event_source, ScopeId scope_id)[]
        {
            (2, "some_source", "bcb87bbf-f495-4f72-9795-d5e8864add5f")
        }).GetAwaiter().GetResult();
        with_event_handlers_filtering_number_of_event_types((true, 0, "bcb87bbf-f495-4f72-9795-d5e8864add5f", true, false));
        event_handler = event_handlers_to_run.First();
    };

    Because of = () =>
    {
        exception = Catch.Exception(() => run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(
            (2, "some_source", "bcb87bbf-f495-4f72-9795-d5e8864add5f")).GetAwaiter().GetResult());
    };

    It should_fail_when_starting = () => exception.ShouldNotBeNull();

}