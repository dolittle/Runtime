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
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Integration.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

namespace Integration.Benchmarks.Events.Processing.EventHandlers;


/// <summary>
/// Benchmarks for Event Handlers.
/// </summary>
public class FastEventHandler : JobBase
{
    IEventStore _eventStore;
    IEventHandlers _eventHandlers;
    IEventHandlerFactory _eventHandlerFactory;
    IEnumerable<IEventHandler> _eventHandlersToRun;
    ArtifactId[] _eventTypes;
    Mock<ReverseCallDispatcher> _dispatcher;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _eventHandlers = services.GetRequiredService<IEventHandlers>();
        _eventHandlerFactory = services.GetRequiredService<IEventHandlerFactory>();
        
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
            _eventStore.Commit(new UncommittedEvents(uncommittedEvents), Runtime.CreateExecutionContextFor(tenant)).GetAwaiter().GetResult();
        }
    }
        
    [IterationSetup]
    public void IterationSetup()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var numEventsProcessed = 0;
        _dispatcher = new Mock<ReverseCallDispatcher>();
        _dispatcher
            .Setup(_ => _.Reject(It.IsAny<EventHandlerRegistrationResponse>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _dispatcher
            .Setup(_ => _.Accept(It.IsAny<EventHandlerRegistrationResponse>(), It.IsAny<CancellationToken>()))
            .Returns(tcs.Task);
        _dispatcher
            .Setup(_ => _.Call(It.IsAny<HandleEventRequest>(), It.IsAny<Dolittle.Runtime.Execution.ExecutionContext>(), It.IsAny<CancellationToken>()))
            .Returns<HandleEventRequest, Dolittle.Runtime.Execution.ExecutionContext, CancellationToken>((request, _, __) =>
            {
                Interlocked.Add(ref numEventsProcessed, 1);
                var response = new EventHandlerResponse();
                if (numEventsProcessed == NumberEventsToProcess)
                {
                    tcs.SetResult();
                }
                return Task.FromResult(response);
            });

        var eventHandlers = new List<IEventHandler>();
        eventHandlers.AddRange(Enumerable.Range(0, EventHandlers).Select(_ => _eventHandlerFactory.CreateFast(
            new EventHandlerRegistrationArguments(Runtime.CreateExecutionContextFor("d9fd643f-ce74-4ae5-b706-b76859fd8827"), Guid.NewGuid(), _eventTypes, Partitioned, ScopeId.Default),
            _dispatcher.Object,
            CancellationToken.None)));
        _eventHandlersToRun = eventHandlers;
    }
    
    [IterationCleanup]
    public void IterationCleanup()
    {
        foreach (var eventHandler in _eventHandlersToRun)
        {
            eventHandler.Dispose();
        }
    }
    
    /// <inheritdoc />
    [Params(1, 2)] // TODO: Adding 10 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public override int NumTenants { get; set; }
    
    /// <summary>
    /// Gets the number of simultaneously running event handlers.
    /// </summary>
    [Params(1, 2, 5)] // TODO: Adding 10 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public int EventHandlers { get; set; }

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
    [Params(1, 10, 100)] // TODO: Adding 100 here results in really slow benchmarks. Let's adding 100 when we've made this blazingly fast :) 
    public int Events { get; set; }
    
    int NumberEventsToProcess => Events * EventTypes * NumTenants * EventHandlers;

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public Task RegisteringAndProcessing()
    {
        return Task.WhenAll(_eventHandlersToRun.Select(eventHandler => _eventHandlers.RegisterAndStart(
            eventHandler,
            (_, _) => _dispatcher.Object.Reject(new EventHandlerRegistrationResponse(), CancellationToken.None),
            CancellationToken.None)));
    }
    
    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
