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
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Integration.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using StreamProcessor = Dolittle.Runtime.Events.Processing.Streams.StreamProcessor;

namespace Integration.Benchmarks.Events.Processing.EventHandlers;


/// <summary>
/// Benchmarks for Event Handler filters. How long it takes to write events to streams.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable. IterationCleanup fixes it.
public class Filter : JobBase
#pragma warning restore CA1001
{
    IEventStore _eventStore = null!;
    IStreamProcessors _streamProcessors = null!;
    ArtifactId[] _eventTypes = null!;
    IEnumerable<StreamProcessor> _streamProcessorsToRun = null!;
    ILoggerFactory _loggerFactor = null!;
    CancellationTokenSource _stopStreamProcessorSource = null!;
    Func<TenantId, IWriteEventsToStreams> _getEventsToStreamsWriter = null!;
    
    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _streamProcessors = services.GetRequiredService<IStreamProcessors>();
        _eventTypes = Enumerable.Range(0, EventTypes).Select(_ => new ArtifactId(Guid.NewGuid())).ToArray();
        _getEventsToStreamsWriter = services.GetRequiredService<Func<TenantId, IWriteEventsToStreams>>();
        _loggerFactor = services.GetRequiredService<ILoggerFactory>();

        var uncommittedEvents = new List<UncommittedEvent>();
        foreach (var eventType in _eventTypes)
        {
            foreach (var _ in Enumerable.Range(0, Events))
            {
                uncommittedEvents.Add(new UncommittedEvent("some event source", new Artifact(eventType, ArtifactGeneration.First), false, "{\"hello\": 42}"));
            }
        }
        foreach (var tenant in ConfiguredTenants)
        {
            _eventStore.Commit(new UncommittedEvents(uncommittedEvents), Runtime.CreateExecutionContextFor(tenant)).GetAwaiter().GetResult();
        }
    }
        
    [IterationSetup]
    public void IterationSetup()
    {
        _stopStreamProcessorSource = new CancellationTokenSource();
        var numEventsProcessed = 0;
        _streamProcessorsToRun = Enumerable.Range(0, EventHandlerFilters).Select(_ =>
        {
            var eventHandlerId = new EventProcessorId(Guid.NewGuid());
            return _streamProcessors.TryCreateAndRegister(
                ScopeId.Default,
                eventHandlerId,
                "Filter",
                new EventLogStreamDefinition(),
                tenant => new TypeFilterWithEventSourcePartition(
                    ScopeId.Default,
                    new TypeFilterWithEventSourcePartitionDefinition(StreamId.EventLog, eventHandlerId.Value, _eventTypes, Partitioned),
                    GetEventWriterForTenant(tenant),
                    _loggerFactor.CreateLogger<TypeFilterWithEventSourcePartition>()),
                Runtime.CreateExecutionContextFor("e3921d20-da26-422e-bf13-e959a5f505ef"),
                _stopStreamProcessorSource.Token).Result;
        }).ToArray()!;

        IWriteEventsToStreams GetEventWriterForTenant(TenantId tenant)
        {
            var writer = _getEventsToStreamsWriter(tenant);
            var wrappedWriter = new Mock<IWriteEventsToStreams>();

            wrappedWriter
                .Setup(_ => _.Write(It.IsAny<CommittedEvent>(), It.IsAny<ScopeId>(), It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<CommittedEvent, ScopeId, StreamId, PartitionId, CancellationToken>(async (@event, scope, stream, partition, cancellation) =>
                {
                    Interlocked.Add(ref numEventsProcessed, 1);
                    await writer.Write(@event, scope, stream, partition, cancellation).ConfigureAwait(false);
                    if (numEventsProcessed == NumberEventsToProcess)
                    {
                        _stopStreamProcessorSource.Cancel();
                    }
                });
            return wrappedWriter.Object;
        }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _stopStreamProcessorSource?.Dispose();
    }
    
    /// <inheritdoc />
    [Params(1, 2, 5)]
    public override int NumTenants { get; set; }
    
    /// <summary>
    /// Gets the number of simultaneously running event handlers.
    /// </summary>
    [Params(1, 2, 5)]
    public int EventHandlerFilters { get; set; }

    /// <summary>
    /// Gets the value indicating whether performing benchmark on a partitioned event handler or not.
    /// </summary>
    // [Params(false, true)] TODO: We can maybe enable this in the future, but as of now it seems that the performance is the same.
    public bool Partitioned { get; set; } = true;

    /// <summary>
    /// Gets the number of event types.
    /// </summary>
    // [Params(1, 10)] TODO: We can maybe enable this in the future, but as of now it seems that the performance depends on the amount of events processed.
    public int EventTypes { get; set; } = 1;
    
    /// <summary>
    /// Gets the number of events committed per configured event type.
    /// </summary>
    [Params(1, 10)] // TODO: Adding 100 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public int Events { get; set; }
    
    int NumberEventsToProcess => Events * EventTypes * NumTenants * EventHandlerFilters;

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task RegisteringAndProcessing()
    {
        await Task.WhenAll(_streamProcessorsToRun.Select(_ => _.Initialize())).ConfigureAwait(false);
        await Task.WhenAll(_streamProcessorsToRun.Select(_ => _.Start())).ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
