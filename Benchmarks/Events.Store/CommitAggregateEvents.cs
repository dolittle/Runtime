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
    IEventStoreService _eventStoreService;
    ExecutionContext _executionContext;
    UncommittedAggregateEvents _eventsToCommit;
    
    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStoreService = services.GetRequiredService<IEventStoreService>();
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
        _eventsToCommit = new UncommittedAggregateEvents(
            new EventSourceId("46c4de33-9a60-4465-97ab-a2a7f5b7e6a3"),
            new Artifact(new ArtifactId(Guid.NewGuid()), ArtifactGeneration.First),
            AggregateRootVersion.Initial,
            _eventsToCommit);
    }
    
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
            var result = await _eventStoreService.TryCommitForAggregate(
                new UncommittedAggregateEvents(
                    _eventsToCommit.EventSource,
                    _eventsToCommit.AggregateRoot,
                    _eventsToCommit.ExpectedAggregateRootVersion + (uint)n,
                    new UncommittedEvents(new[] { _eventsToCommit[n] })),
                _executionContext,
                CancellationToken.None);
            if (!result.Success)
            {
                throw result.Exception;
            }
        }
    }

    /// <summary>
    /// Commits the events in a single batch.
    /// </summary>
    [Benchmark]
    public async Task CommitEventsInBatch()
    {
        var result = await _eventStoreService.TryCommitForAggregate(
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
