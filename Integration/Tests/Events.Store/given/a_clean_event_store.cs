// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Tests.Events.Store.given;

class a_clean_event_store : a_runtime_with_a_single_tenant
{
    protected static IEventStore event_store;
    protected static IStreams streams;
    protected static IAggregateRoots aggregate_roots;
    protected static EventLogSequenceNumber current_sequence_number;
    
    Establish context = () =>
    {
        current_sequence_number = EventLogSequenceNumber.Initial;
        event_store = runtime.Host.Services.GetRequiredService<Func<TenantId, IEventStore>>()(execution_context.Tenant);
        streams = runtime.Host.Services.GetRequiredService<Func<TenantId, IStreams>>()(execution_context.Tenant);
        aggregate_roots = runtime.Host.Services.GetRequiredService<Func<TenantId, IAggregateRoots>>()(execution_context.Tenant);
    };
}