// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.given;

class single_tenant_and_event_handlers : EventHandlers.given.single_tenant_and_event_handlers
{
    protected static void with_event_handlers_filtering_number_of_event_types(params (int max_event_types_to_filter, bool fast, bool implicitFilter)[] event_handler_infos)
        => with_event_handlers_filtering_number_of_event_types(event_handler_infos
            .Select(_ => (true, _.max_event_types_to_filter, ScopeId.Default, _.fast, _.implicitFilter))
            .ToArray());

}


    