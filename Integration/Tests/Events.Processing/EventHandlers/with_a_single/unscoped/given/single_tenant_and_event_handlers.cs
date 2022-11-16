// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

using System.Linq;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;

namespace Integration.Tests.Events.Processing.EventHandlers.with_a_single.unscoped.given;

class single_tenant_and_event_handlers : EventHandlers.given.single_tenant_and_event_handlers
{
    protected static void commit_events_for_each_event_type(params (int number_of_events, EventSourceId event_source_id)[] commit)
        => commit_events_for_each_event_type(commit.Select(_ => (_.number_of_events, _.event_source_id, ScopeId.Default))).GetAwaiter().GetResult();

    protected static void commit_events_after_starting_event_handler(params (int number_of_events, EventSourceId event_source_id)[] commit)
        => run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(commit
            .Select(_ => (_.number_of_events, _.event_source_id, ScopeId.Default))
            .ToArray()).GetAwaiter().GetResult();
}


    