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
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

namespace Benchmarks.Events.Processing;


/// <summary>
/// Benchmarks for Event Handlers.
/// </summary>
public class EventHandler : JobBase
{
    IEventStore _eventStore;
    ExecutionContext _executionContext;
    IEventHandlers _eventHandlers;
    IEventHandlerFactory _eventHandlerFactory;
    IEventHandler _eventHandler;
    ArtifactId[] _eventTypes;
    Mock<ReverseCallDispatcher> _dispatcher;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _eventStore = services.GetRequiredService<IEventStore>();
        _executionContext = CreateExecutionContextFor(ConfiguredTenants.First());
        _eventHandlers = services.GetRequiredService<IEventHandlers>();
        _eventHandlerFactory = services.GetRequiredService<IEventHandlerFactory>();
        
        _eventTypes = Enumerable.Range(0, EventTypes).Select(_ => new ArtifactId(Guid.NewGuid())).ToArray();
        var uncommittedEvents = new List<UncommittedEvent>();
        foreach (var eventType in _eventTypes)
        {
            foreach (var _ in Enumerable.Range(0, EventsPerType))
            {
                uncommittedEvents.Add(new UncommittedEvent("some event source", new Artifact(eventType, ArtifactGeneration.First), false, "{\"hello\": 42}"));
            }
        }
        _eventStore.Commit(new UncommittedEvents(uncommittedEvents), _executionContext).GetAwaiter().GetResult();
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
            .Setup(_ => _.Call(It.IsAny<HandleEventRequest>(), It.IsAny<ExecutionContext>(), It.IsAny<CancellationToken>()))
            .Returns<HandleEventRequest, ExecutionContext, CancellationToken>((request, _, __) =>
            {
                var response = new EventHandlerResponse();
                if (++numEventsProcessed == NumberEventsToProcess)
                {
                    tcs.SetResult();
                }
                return Task.FromResult(response);
            });
        _eventHandler = _eventHandlerFactory.Create(
            new EventHandlerRegistrationArguments(_executionContext, Guid.NewGuid(), _eventTypes, Partitioned, ScopeId.Default),
            _dispatcher.Object,
            CancellationToken.None);
    }
    
    [IterationCleanup]
    public void IterationCleanup()
    {
        _eventHandler.Dispose();
    }
    
    /// <summary>
    /// Gets the value indicating whether performing benchmark on a partitioned event handler or not.
    /// </summary>
    [Params(false, true)]
    public bool Partitioned { get; set; }
    
    /// <summary>
    /// Gets the number of events committed per configured event type.
    /// </summary>
    [Params(1, 10)]
    public int EventsPerType { get; set; }
    
    /// <summary>
    /// Gets the number of event types.
    /// </summary>
    [Params(1, 10)]
    public int EventTypes { get; set; }

    int NumberEventsToProcess => EventsPerType * EventTypes;

    /// <summary>
    /// Commits the events one-by-one in a loop.
    /// </summary>
    [Benchmark]
    public Task RegisteringAndProcessing()
    {
        return _eventHandlers.RegisterAndStart(
            _eventHandler,
            (_, _) => _dispatcher.Object.Reject(new EventHandlerRegistrationResponse(), CancellationToken.None),
            CancellationToken.None);
    }
    
    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
