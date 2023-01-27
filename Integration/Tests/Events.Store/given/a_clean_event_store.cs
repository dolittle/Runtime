// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;

namespace Integration.Tests.Events.Store.given;

public class a_clean_event_store : a_runtime_with_a_single_tenant
{
    protected static readonly FilterDefinition<Event> all_events_filter = Builders<Event>.Filter.Empty;
    protected static IEventStore event_store;
    protected static IEventContentConverter event_content_converter;
    protected static IStreams streams;
    protected static IAggregateRoots aggregate_roots;
    protected static IEventLogStream event_log_stream;
    protected static IWriteEventsToStreams events_to_streams_writer;
    

    Establish context = () =>
    {
        event_store = runtime.Host.Services.GetRequiredService<IEventStore>();
        event_content_converter = runtime.Host.Services.GetRequiredService<IEventContentConverter>();
        streams = runtime.Host.Services.GetRequiredService<Func<TenantId, IStreams>>()(execution_context.Tenant);
        aggregate_roots = runtime.Host.Services.GetRequiredService<Func<TenantId, IAggregateRoots>>()(execution_context.Tenant);
        event_log_stream = runtime.Host.Services.GetRequiredService<Func<TenantId, IEventLogStream>>()(execution_context.Tenant);
        events_to_streams_writer = runtime.Host.Services.GetRequiredService<Func<TenantId, IWriteEventsToStreams>>()(execution_context.Tenant);
    };
    
    protected static void number_of_events_stored_should_be(int num_events) => streams.DefaultEventLog.CountDocuments(Builders<Event>.Filter.Empty).Should().Be(num_events);
}