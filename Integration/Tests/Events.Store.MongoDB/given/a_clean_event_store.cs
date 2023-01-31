// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;

namespace Integration.Tests.Events.Store.MongoDB.given;

public class a_clean_event_store : a_runtime_with_a_single_tenant
{
    protected static IStreamProcessorStateRepository stream_processor_state_repository;
    

    Establish context = () =>
    {
        stream_processor_state_repository = runtime.Host.Services.GetRequiredService<Func<TenantId, IStreamProcessorStateRepository>>()(execution_context.Tenant);
    };
}