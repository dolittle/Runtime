// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
/// Benchmarks for fetching aggregate events from the Event Store.
/// </summary>
public class FetchAggregateEvents : JobBase
{
    IEventStore _eventStore;
    ExecutionContext _executionContext;
    ArtifactId _aggregateRoot;
    EventSourceId _eventSource;
    
    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _executionContext = CreateExecutionContextFor(ConfiguredTenants.First());

        _aggregateRoot = new ArtifactId(Guid.NewGuid());
        _eventSource = new EventSourceId("fe7d58f0-2f4c-4736-8f9e-68dd7dfaec82");

        if (CommittedEvents < 1)
        {
            return;
        }

        for (var n = 0; n < CommittedEvents; n++)
        {
            Commit(
                _eventStore,
                new UncommittedAggregateEvents(
                    _eventSource,
                    new Artifact(_aggregateRoot, ArtifactGeneration.First),
                    (uint)n,
                    new[]
                    {
                        new UncommittedEvent(
                            _eventSource,
                            new Artifact(new ArtifactId(Guid.Parse("f029f857-f9e6-45d4-b3ab-4b9b0d798f2c")), ArtifactGeneration.First),
                            false,
                            "{ \"hello\": \"world\" }")
                    }),
                _executionContext).GetAwaiter().GetResult();

            if (UnrelatedEventsRatio < 1)
            {
                continue;
            }

            for (var m = 0; m < UnrelatedEventsRatio; m++)
            {
                Commit(
                    _eventStore,
                    new UncommittedEvents(new[]
                    {
                        new UncommittedEvent(
                            _eventSource,
                            new Artifact(new ArtifactId(Guid.Parse("9f90d6df-0711-4ac9-bf1c-51d6090733e9")), ArtifactGeneration.First),
                            false,
                            "{ \"hello\": \"world\" }")
                    }),
                    _executionContext).GetAwaiter().GetResult();
            }
        }
    }
    
    /// <summary>
    /// Gets the number of events that has been committed for the aggregate root.
    /// </summary>
    [Params(0, 1, 10, 50, 100)]
    public int CommittedEvents { get; set; }
    
    /// <summary>
    /// Gets the ratio of other events unrelated to the aggregate root that have been committed.
    /// </summary>
    [Params(0, 1, 10)]
    public int UnrelatedEventsRatio { get; set; }

    /// <summary>
    /// Fetches the events for the aggregate root.
    /// </summary>
    [Benchmark]
    public async Task FetchEvents()
    {
        await FetchForAggregate(
            _eventStore,
            _aggregateRoot,
            _eventSource,
            _executionContext);
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
