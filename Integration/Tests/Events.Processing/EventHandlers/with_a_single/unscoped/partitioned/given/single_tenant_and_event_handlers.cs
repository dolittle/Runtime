// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Integration.Tests.Events.Processing.EventHandlers.given;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.partitioned.given;

class single_tenant_and_event_handlers : unscoped.given.single_tenant_and_event_handlers
{
    protected static void expect_stream_definition(IEventHandler event_handler, int max_handled_event_types)
        => expect_correct_stream_definition(event_handler, partitioned: true, public_stream: false, max_handled_event_types: max_handled_event_types);
    
    protected static void expect_stream_processor_state(IEventHandler event_handler, int num_events_to_handle, failing_partitioned_state failing_partitioned_state)
        => expect_stream_processor_state(
            event_handler,
            partitioned: true,
            num_events_to_handle: num_events_to_handle,
            failing_partitioned_state: failing_partitioned_state,
            failing_unpartitioned_state: null);
    protected static void with_event_handlers_filtering_number_of_event_types(params (int max_event_types_to_filter, bool fast, bool implicitFilter)[] event_handler_infos)
        => with_event_handlers_filtering_number_of_event_types(event_handler_infos
            .Select(_ => (true, _.max_event_types_to_filter, ScopeId.Default, _.fast, _.implicitFilter))
            .ToArray());

}


    