// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.scoped.partitioned.fast_event_handler.without_implicit_filter;

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
        exception = Catch.Exception(() => commit_events_after_starting_event_handler((2, "some_source")));
    };

    It should_fail_starting_event_handler = () => exception.ShouldNotBeNull();
}