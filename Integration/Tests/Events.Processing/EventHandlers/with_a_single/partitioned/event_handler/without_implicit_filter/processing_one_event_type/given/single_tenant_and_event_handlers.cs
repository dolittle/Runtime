// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.partitioned.event_handler.without_implicit_filter.processing_one_event_type.given;

class single_tenant_and_event_handlers : with_implicit_filter.given.single_tenant_and_event_handlers
{

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


    