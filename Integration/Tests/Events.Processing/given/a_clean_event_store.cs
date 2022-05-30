// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;

namespace Integration.Tests.Events.Processing.given;

class a_clean_event_store : a_runtime_with_a_single_tenant
{
    protected static readonly FilterDefinition<Event> all_events_filter = Builders<Event>.Filter.Empty;
    protected static IEventStore event_store;
    protected static IEventContentConverter event_content_converter;
    protected static IStreams streams;
    protected static IEventFetchers event_fetchers;
    protected static IResilientStreamProcessorStateRepository stream_processor_states;
    protected static IStreamDefinitionRepository stream_definition_repository;


    Establish context = () =>
    {
        event_store = runtime.Host.Services.GetRequiredService<Func<TenantId, IEventStore>>()(execution_context.Tenant);
        event_content_converter = runtime.Host.Services.GetRequiredService<IEventContentConverter>();
        streams = runtime.Host.Services.GetRequiredService<Func<TenantId, IStreams>>()(execution_context.Tenant);
        event_fetchers = runtime.Host.Services.GetRequiredService<Func<TenantId, IEventFetchers>>()(tenant);
        stream_processor_states = runtime.Host.Services.GetRequiredService<Func<TenantId, IResilientStreamProcessorStateRepository>>()(tenant);
        stream_definition_repository = runtime.Host.Services.GetRequiredService<Func<TenantId, IStreamDefinitionRepository>>()(tenant);
    };
}