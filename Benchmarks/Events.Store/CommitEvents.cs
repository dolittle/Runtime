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

/// <summary>
/// Benchmarks for committing events to the Event Store.
/// </summary>
public class CommitEvents : JobBase
{
    IEventStore _eventStore;
    ExecutionContext _executionContext;
    UncommittedEvents _eventsToCommit;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _executionContext = CreateExecutionContextFor(ConfiguredTenants.First());

        var events = new List<UncommittedEvent>();
        for (var n = 0; n < EventsToCommit; n++)
        {
            events.Add(new UncommittedEvent(
                new EventSourceId("8453f4d4-861a-4042-a4e3-c1abf1c8eadd"),
                new Artifact(new ArtifactId(Guid.Parse("752f88c9-70f0-4ffe-82b6-a69dcc96672e")), ArtifactGeneration.First),
                false,
                "{ \"hello\": \"world\" }"));
        }
        _eventsToCommit = new UncommittedEvents(events);

        if (PreExistingEvents < 1)
        {
            return;
        }
        
        var preExistingEvents = new List<UncommittedEvent>();
        for (var n = 0; n < PreExistingEvents; n++)
        {
            preExistingEvents.Add(new UncommittedEvent(
                new EventSourceId("8453f4d4-861a-4042-a4e3-c1abf1c8eadd"),
                new Artifact(new ArtifactId(Guid.Parse("752f88c9-70f0-4ffe-82b6-a69dcc96672e")), ArtifactGeneration.First),
                false,
                "{ \"hello\": \"world\" }"));
        }
        
        Commit(
            _eventStore,
            new UncommittedEvents(preExistingEvents),
            _executionContext).GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Gets the number of events that have been committed to the Event Store before the benchmarking commits.
    /// </summary>
    [Params(0, 100, 1000)]
    public int PreExistingEvents { get; set; }
    
    /// <summary>
    /// Gets the number of events to be committed in the benchmarks.
    /// </summary>
    [Params(1, 10, 50, 100)]
    public int EventsToCommit { get; set; }

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            await Commit(_eventStore, new UncommittedEvents(new[]
            {
                _eventsToCommit[n]
            }), _executionContext);
        }
    }

    /// <summary>
    /// Commits the events in a single batch.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInBatch()
    {
        await Commit(_eventStore, _eventsToCommit, _executionContext);
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}