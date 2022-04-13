// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Integration.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Benchmarks.Events.Store;

/// <summary>
/// Benchmarks for committing events to the Event Store.
/// </summary>
public class CommitEvents : JobBase
{
    IEventStore _eventStore;
    Dolittle.Runtime.Execution.ExecutionContext _executionContext;
    UncommittedEvents _eventsToCommit;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _executionContext = Runtime.CreateExecutionContextFor(ConfiguredTenants.First());

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
        
        _eventStore.Commit(new UncommittedEvents(preExistingEvents), _executionContext).GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Gets the number of events that have been committed to the Event Store before the benchmarking commits.
    /// </summary>
    [Params(0, 100)]
    public int PreExistingEvents { get; set; }
    
    /// <summary>
    /// Gets the number of events to be committed in the benchmarks.
    /// </summary>
    [Params(1, 100)]
    public int EventsToCommit { get; set; }

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            await _eventStore.Commit( new UncommittedEvents(new[]
            {
                _eventsToCommit[n]
            }), _executionContext).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Commits the events in a single batch.
    /// </summary>
    [Benchmark]
    public Task CommitEventsInBatch()
    {
        return _eventStore.Commit(_eventsToCommit, _executionContext);
    }
    
    /// <summary>
    /// Commits the events in a single batch.
    /// </summary>
    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    public async Task CommitEventsInParallel(int ParallelCommits)
    {
        var tasks = new List<Task>();
        foreach (var i in Enumerable.Range(0, ParallelCommits))
        {
            tasks.Add(_eventStore.Commit(_eventsToCommit, _executionContext));   
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
