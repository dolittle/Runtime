// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.event_handler.without_implicit_filter.given;

class single_tenant_and_event_handlers : event_handler.given.single_tenant_and_event_handlers
{

    protected static void with_event_handlers_filtering_number_of_event_types(params int[] max_event_types_to_filter)
        => with_event_handlers_filtering_number_of_event_types(max_event_types_to_filter
            .Select(_ => (_, false))
            .ToArray());

}


    