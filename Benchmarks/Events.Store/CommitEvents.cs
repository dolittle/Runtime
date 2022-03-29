// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Services;
using Microsoft.Extensions.DependencyInjection;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Benchmarks.Events.Store;

public class CommitEvents : JobBase
{
    IEventStoreService _eventStoreService;
    ExecutionContext _executionContext;
    UncommittedEvents _eventsToCommit;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStoreService = services.GetRequiredService<IEventStoreService>();
        _executionContext = CreateExecutionContextFor(ConfiguredTenants.First());

        var events = new List<UncommittedEvent>();
        for (var n = 0; n < EventsToCommit; n++)
        {
            events.Add(new UncommittedEvent(
                new EventSourceId("8453f4d4-861a-4042-a4e3-c1abf1c8eadd"),
                new Artifact(new ArtifactId(Guid.Parse("752f88c9-70f0-4ffe-82b6-a69dcc96672e")), ArtifactGeneration.First),
                Public,
                "{ \"hello\": \"world\" }"));
        }
        _eventsToCommit = new UncommittedEvents(events);
    }
    
    [Params(false, true)]
    public bool Public { get; set; }
    
    [Params(1, 10, 50, 100)]
    public int EventsToCommit { get; set; }

    [Benchmark]
    public async Task CommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            var result = await _eventStoreService.TryCommit(
                new UncommittedEvents(new[] { _eventsToCommit[n] }),
                _executionContext,
                CancellationToken.None);
            if (!result.Success)
            {
                throw result.Exception;
            }
        }
    }

    [Benchmark]
    public async Task CommitEventsInBatch()
    {
        var result = await _eventStoreService.TryCommit(
            _eventsToCommit,
            _executionContext,
            CancellationToken.None);
        if (!result.Success)
        {
            throw result.Exception;
        }
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
