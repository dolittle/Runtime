// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Services;
using Microsoft.Extensions.DependencyInjection;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Benchmarks.Events.Store;

/// <summary>
/// Benchmarks for committing aggregate events to the Event Store.
/// </summary>
public class CommitAggregateEvents : JobBase
{
    IEventStore _eventStore;
    ExecutionContext _executionContext;
    UncommittedAggregateEvents _eventsToCommit;
    
    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _executionContext = CreateExecutionContextFor(ConfiguredTenants.First());

        var events = new List<UncommittedEvent>();
        for (var n = 0; n < EventsToCommit; n++)
        {
            events.Add(new UncommittedEvent(
                new EventSourceId("46c4de33-9a60-4465-97ab-a2a7f5b7e6a3"),
                new Artifact(new ArtifactId(Guid.Parse("08db4b0a-3724-444f-9968-ada44922fb78")), ArtifactGeneration.First),
                false,
                "{ \"hello\": \"world\" }"));
        }

        _eventsToCommit = new UncommittedAggregateEvents(
            new EventSourceId("46c4de33-9a60-4465-97ab-a2a7f5b7e6a3"),
            new Artifact(new ArtifactId(Guid.Parse("1ad7a5dc-12e9-493a-ba10-714c88be4da7")), ArtifactGeneration.First),
            AggregateRootVersion.Initial,
            events);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        var eventSource = new EventSourceId("46c4de33-9a60-4465-97ab-a2a7f5b7e6a3");
        var aggregateRoot = new Artifact(new ArtifactId(Guid.NewGuid()), ArtifactGeneration.First);
        _eventsToCommit = new UncommittedAggregateEvents(
            eventSource,
            aggregateRoot,
            AggregateRootVersion.Initial + (ulong)PreExistingEvents,
            _eventsToCommit);

        if (PreExistingEvents > 0)
        {
            var events = new List<UncommittedEvent>();
            for (var n = 0; n < PreExistingEvents; n++)
            {
                events.Add(new UncommittedEvent(
                    new EventSourceId("46c4de33-9a60-4465-97ab-a2a7f5b7e6a3"),
                    new Artifact(new ArtifactId(Guid.Parse("08db4b0a-3724-444f-9968-ada44922fb78")), ArtifactGeneration.First),
                    false,
                    "{ \"hello\": \"world\" }"));
            }
            var preExistingEvents = new UncommittedAggregateEvents(
                eventSource,
                aggregateRoot,
                AggregateRootVersion.Initial,
                events);
            
            _eventStore.Commit(preExistingEvents, _executionContext).GetAwaiter().GetResult();
        }
    }
    
    /// <summary>
    /// Gets the number of events to be committed in the benchmarks.
    /// </summary>
    [Params(1, 100)]
    public int EventsToCommit { get; set; }

    /// <summary>
    /// Gets the number of events that have been committed to the Event Store before the benchmarking commits.
    /// </summary>
    [Params(0, 1, 100)]
    public int PreExistingEvents { get; set; }

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            await _eventStore.Commit(
                new UncommittedAggregateEvents(
                    _eventsToCommit.EventSource,
                    _eventsToCommit.AggregateRoot,
                    _eventsToCommit.ExpectedAggregateRootVersion + (uint) n,
                    new UncommittedEvents(new[]
                    {
                        _eventsToCommit[n]
                    })),
                _executionContext).ConfigureAwait(false);
        }
    }
    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task FetchAndCommitEventsInLoop()
    {
        for (var n = 0; n < EventsToCommit; n++)
        {
            await _eventStore.FetchForAggregate(_eventsToCommit.AggregateRoot.Id, _eventsToCommit.EventSource, _executionContext).ConfigureAwait(false);
            await _eventStore.Commit(
                new UncommittedAggregateEvents(
                    _eventsToCommit.EventSource,
                    _eventsToCommit.AggregateRoot,
                    _eventsToCommit.ExpectedAggregateRootVersion + (uint) n,
                    new UncommittedEvents(new[]
                    {
                        _eventsToCommit[n]
                    })),
                _executionContext).ConfigureAwait(false);
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
    public async Task FetchCommitEventsInBatch()
    {
        await _eventStore.FetchForAggregate(_eventsToCommit.AggregateRoot.Id, _eventsToCommit.EventSource, _executionContext).ConfigureAwait(false);
        await _eventStore.Commit(_eventsToCommit, _executionContext).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
