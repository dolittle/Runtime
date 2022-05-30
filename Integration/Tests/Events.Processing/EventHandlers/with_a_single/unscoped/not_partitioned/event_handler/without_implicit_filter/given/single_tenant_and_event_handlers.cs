// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Integration.Tests.Events.Processing.EventHandlers.given;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.not_partitioned.event_handler.without_implicit_filter.given;

class single_tenant_and_event_handlers : event_handler.given.single_tenant_and_event_handlers
{
    protected static void expect_stream_processor_state_without_failure(IEventHandler event_handler, int num_events_to_handle)
        => expect_stream_processor_state(event_handler, false, false, num_events_to_handle, null!);
    
    protected static void expect_stream_processor_state_with_failure(IEventHandler event_handler, failing_unpartitioned_state failing_unpartitioned_state)
        => expect_stream_processor_state(event_handler, false, false, (int)failing_unpartitioned_state.failing_position.Value, failing_unpartitioned_state);
    
    protected static void with_event_handlers_filtering_number_of_event_types(params int[] max_event_types_to_filter)
        => with_event_handlers_filtering_number_of_event_types(max_event_types_to_filter
            .Select(_ => (_, false))
            .ToArray());

}


    