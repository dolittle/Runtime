// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.partitioned.fast_event_handler.given;

class single_tenant_and_event_handlers : partitioned.given.single_tenant_and_event_handlers
{
    protected static void with_event_handlers_filtering_number_of_event_types(params (int max_event_types_to_filter, bool implicitFilter)[] event_handler_infos)
        => with_event_handlers_filtering_number_of_event_types(event_handler_infos
            .Select(_ => (_.max_event_types_to_filter, true, _.implicitFilter))
            .ToArray());

}


    