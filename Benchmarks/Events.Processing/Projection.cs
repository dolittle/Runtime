// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Benchmarks.Events.Store;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ProjectionEventSelector = Dolittle.Runtime.Projections.Store.Definition.ProjectionEventSelector;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionRequest,
    Dolittle.Runtime.Events.Processing.Contracts.ProjectionResponse>;

namespace Benchmarks.Events.Processing;

/// <summary>
/// Benchmarks for Projections.
/// </summary>
public class Projection : JobBase
{
    IEventStore _eventStore;
    IProjections _projections;
    ArtifactId[] _eventTypes;
    IEnumerable<IProjection> _projectionsToRun;
    Mock<ReverseCallDispatcher> _dispatcher;
    CancellationTokenSource _cancellationTokenSource;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _projections = services.GetRequiredService<IProjections>();
        _eventTypes = Enumerable.Range(0, EventTypes).Select(_ => new ArtifactId(Guid.NewGuid())).ToArray();
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
            _eventStore.Commit(new UncommittedEvents(uncommittedEvents), CreateExecutionContextFor(tenant)).GetAwaiter().GetResult();
        }
    }
        
    [IterationSetup]
    public void IterationSetup()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var numEventsProcessed = 0;
        _dispatcher = new Mock<ReverseCallDispatcher>();
        _dispatcher
            .Setup(_ => _.Reject(It.IsAny<ProjectionRegistrationResponse>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _dispatcher
            .Setup(_ => _.Accept(It.IsAny<ProjectionRegistrationResponse>(), It.IsAny<CancellationToken>()))
            .Returns(tcs.Task);
        _dispatcher
            .Setup(_ => _.Call(It.IsAny<ProjectionRequest>(), It.IsAny<ExecutionContext>(), It.IsAny<CancellationToken>()))
            .Returns<ProjectionRequest, ExecutionContext, CancellationToken>((request, _, __) =>
            {
                Interlocked.Add(ref numEventsProcessed, 1);
                var response = new ProjectionResponse{Replace = new ProjectionReplaceResponse{State = request.CurrentState.State}};
                if (numEventsProcessed == NumberEventsToProcess)
                {
                    tcs.SetResult();
                }
                return Task.FromResult(response);
            });

        _projectionsToRun = Enumerable.Range(0, Projections).Select(_ => new Dolittle.Runtime.Events.Processing.Projections.Projection(
            _dispatcher.Object,
            new ProjectionDefinition(
                Guid.NewGuid(),
                ScopeId.Default,
                _eventTypes.Select(ProjectionEventSelector.EventSourceId),
                new ProjectionState("{\"hello\": 42}"),
                new ProjectionCopySpecification(CopyToMongoDBSpecification.Default)),
            "some alias",
            false)).ToArray();
    }
    
    [IterationCleanup]
    public void IterationCleanup()
    {
        _cancellationTokenSource?.Dispose();
    }
    
    /// <inheritdoc />
    [Params(1, 2, 5)] // TODO: Adding 10 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public override int NumTenants { get; set; }
    
    /// <summary>
    /// Gets the number of simultaneously running event handlers.
    /// </summary>
    [Params(1, 2, 5)] // TODO: Adding 10 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public int Projections { get; set; }

    /// <summary>
    /// Gets the number of event types.
    /// </summary>
    // [Params(1, 10)] TODO: We can maybe enable this in the future, but as of now it seems that the performance depends on the amount of events processed.
    public int EventTypes { get; set; } = 1;
    
    /// <summary>
    /// Gets the number of events committed per configured event type.
    /// </summary>
    [Params(1, 10, 100)] 
    public int Events { get; set; }
    
    int NumberEventsToProcess => Events * EventTypes * NumTenants * Projections;

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public async Task RegisteringAndProcessing()
    {
        var x = await Task.WhenAll(_projectionsToRun.Select(projection => _projections.Register(
            projection,
            CreateExecutionContextFor("a2775100-bad1-4260-a97f-13ef9caf9720"),
            _cancellationTokenSource.Token))).ConfigureAwait(false);

        var dispatcherTask = _dispatcher.Object.Accept(new ProjectionRegistrationResponse(), CancellationToken.None);

        var tasks = x.Select(_ => _.Result.Start()).Append(dispatcherTask);
        var taskGroup = new TaskGroup(tasks);

        await taskGroup.WaitForAllCancellingOnFirst(_cancellationTokenSource);
        foreach (var projectionProcessor in x)
        {
            projectionProcessor.Result?.Dispose();
        }
    }
    
    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
