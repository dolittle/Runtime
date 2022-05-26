// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Integration.Tests.Events.Processing.EventHandlers.given;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.fast_event_handler.with_implicit_filter.not_processing_one_event_type.given;

class single_tenant_and_event_handlers : with_implicit_filter.given.single_tenant_and_event_handlers
{
    
    protected static void expect_stream_definition(IEventHandler event_handler)
        => expect_stream_definition(event_handler, 1);

    protected static void expect_stream_processor_state_without_failure(IEventHandler event_handler)
        => expect_stream_processor_state_without_failure(event_handler, committed_events_for_event_types(1).Count());


    protected static IEventHandler setup_event_handler()
    {
        with_event_handlers_filtering_number_of_event_types(new[]
            {
                1
            }
            .Select(_ => (_, true))
            .ToArray());
        return event_handlers_to_run.First();
    }
}


    