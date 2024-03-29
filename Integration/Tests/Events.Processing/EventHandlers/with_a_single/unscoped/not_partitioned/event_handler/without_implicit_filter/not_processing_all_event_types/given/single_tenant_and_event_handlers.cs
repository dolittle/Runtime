// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.event_handler.without_implicit_filter.processing_all_event_types.given;

class single_tenant_and_event_handlers : without_implicit_filter.given.single_tenant_and_event_handlers
{
    protected static void expect_stream_definition(IEventHandler event_handler)
        => expect_stream_definition(event_handler, number_of_event_types);
    
    
    protected static void expect_stream_processor_state_without_failure(IEventHandler event_handler)
        => expect_stream_processor_state_without_failure(event_handler, committed_events.Count);

    protected static IEventHandler setup_event_handler()
    {
        with_event_handlers_filtering_number_of_event_types(new[]
            {
                number_of_event_types
            }
            .Select(_ => _)
            .ToArray());
        return event_handlers_to_run.First();
    }

}


    