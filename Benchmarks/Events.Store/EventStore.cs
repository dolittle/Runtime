// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Services;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Benchmarks.Events.Store;

public class EventStore
{
    IHost _runtimeHost;
    IEventStoreService _eventStoreService;
    
    [GlobalSetup]
    public void StartServer()
    {
        _runtimeHost = Host.CreateDefaultBuilder()
            .UseDolittleServices()
            .ConfigureHostConfiguration(_ => _
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["dolittle:runtime:eventstore:backwardscompatibility:version"] = "V7",
                        
                        ["dolittle:runtime:tenants:445f8ea8-1a6f-40d7-b2fc-796dba92dc44:resources:eventStore:servers:0"] = "localhost",
                        ["dolittle:runtime:tenants:445f8ea8-1a6f-40d7-b2fc-796dba92dc44:resources:eventStore:database"] = "test_store_4",
                        ["dolittle:runtime:platform:microserviceID"] = "0d3e7f3d-bd47-4445-a114-58c0d44f961a",
                        ["dolittle:runtime:platform:environment"] = "Development",
                    }))
            .AddMetrics()
            .AddGrpcHost(EndpointVisibility.Private)
            .AddGrpcHost(EndpointVisibility.Public)
            .AddGrpcHost(EndpointVisibility.Management)
            .AddMetricsHost()
            .AddWebHost()
            .Build();

        _runtimeHost.StartAsync().GetAwaiter().GetResult();

        _eventStoreService = _runtimeHost.Services.GetRequiredService<IEventStoreService>();
    }
    
    [GlobalCleanup]
    public void StopServer()
    {
        _runtimeHost.StopAsync().GetAwaiter().GetResult();
    }
    
    [Benchmark(OperationsPerInvoke = 100)]
    public async Task CommitEvent()
    {
        var result = await _eventStoreService.TryCommit(
                new UncommittedEvents(new[]
                {
                    new UncommittedEvent(new EventSourceId("event-source"), Artifact.New(), false, "{ \"some\": 42 }"),
                }),
                new ExecutionContext(
                    "0d3e7f3d-bd47-4445-a114-58c0d44f961a",
                    TenantId.Development,
                    Version.NotSet,
                    Environment.Development,
                    CorrelationId.New(),
                    Claims.Empty,
                    CultureInfo.InvariantCulture),
                CancellationToken.None);
        if (!result.Success)
        {
            throw result.Exception;
        }
    }

    [Benchmark(OperationsPerInvoke = 1)]
    public async Task CommitAggregateEvent()
    {
        var aggregateId = new ArtifactId(Guid.NewGuid());
        var executionContext = new ExecutionContext(
            "0d3e7f3d-bd47-4445-a114-58c0d44f961a",
            TenantId.Development,
            Version.NotSet,
            Environment.Development,
            CorrelationId.New(),
            Claims.Empty,
            CultureInfo.InvariantCulture);
        var events = await _eventStoreService.TryFetchForAggregate(aggregateId, "event-source", executionContext, CancellationToken.None);
        if (!events.Success)
        {
            throw events.Exception;
        }

        var result = await _eventStoreService.TryCommitForAggregate(
            new UncommittedAggregateEvents(
                "event-source",
                new Artifact(aggregateId, ArtifactGeneration.First),
                new AggregateRootVersion((ulong)events.Result.Count),
                new []
                {
                    new UncommittedEvent(new EventSourceId("event-source"), Artifact.New(), false, "{ \"some\": 42 }"),
                }
                ),
            executionContext,
            CancellationToken.None);
        if (!result.Success)
        {
            throw result.Exception;
        }
    }
}
