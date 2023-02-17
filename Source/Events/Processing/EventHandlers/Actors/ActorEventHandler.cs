// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Failure = Dolittle.Runtime.Protobuf.Failure;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

/// <summary>
/// Represents an event handler in the system.
/// </summary>
/// <remarks>
/// An event handler is a formalized type that consists of a filter and an event processor.
/// The filter filters off of an event log based on the types of events the handler is interested in
/// and puts these into a stream for the filter. From this new stream, the event processor will handle
/// the forwarding to the client for it to handle the event.
/// What sets an event handler apart is that it has a formalization around the stream definition that
/// consists of the events it is interested in, which is defined from the client.
/// </remarks>
public class ActorEventHandler : IEventHandler
{
    readonly ActorSystem _actorSystem;
    readonly ITenants _tenants;
    readonly CreateStreamProcessorActorProps _createStreamProcessorActorProps;
    readonly IStreamDefinitions _streamDefinitions;
    readonly EventHandlerRegistrationArguments _arguments;
    readonly Func<TenantId, IEventProcessor> _eventProcessorForTenant;
    readonly Func<CancellationToken, Task> _acceptRegistration;
    readonly Func<Failure, CancellationToken, Task> _rejectRegistration;
    readonly IMetricsCollector _metrics;
    readonly ExecutionContext _executionContext;
    readonly ILogger _logger;
    readonly CancellationTokenSource _cancellationTokenSource;

    PID? _eventHandlerKindPid;

    bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="EventHandler"/>.
    /// </summary>
    /// <param name="filterStreamProcessors">The <see cref="IFilterStreamProcessors"/>.</param>
    /// <param name="streamDefinitions">The<see cref="IStreamDefinitions" />.</param>
    /// <param name="arguments">Connecting arguments.</param>
    /// <param name="eventProcessorForTenant">The event processor.</param>
    /// <param name="acceptRegistration">Accepts the event handler registration.</param>
    /// <param name="rejectRegistration">Rejects the event handler registration.</param>
    /// <param name="metrics">The collector to use for metrics.</param>
    /// <param name="logger">Logger for logging.</param>
    /// <param name="executionContext">The execution context for the event handler.</param>
    /// <param name="cancellationToken">Cancellation token that can cancel the hierarchy.</param>
    /// <param name="actorSystem"></param>
    /// <param name="tenants"></param>
    /// <param name="createStreamProcessorActorProps"></param>
    public ActorEventHandler(
        IStreamDefinitions streamDefinitions,
        EventHandlerRegistrationArguments arguments,
        Func<TenantId, IEventProcessor> eventProcessorForTenant,
        Func<CancellationToken, Task> acceptRegistration,
        Func<Failure, CancellationToken, Task> rejectRegistration,
        IMetricsCollector metrics,
        ILogger logger,
        ExecutionContext executionContext,
        ActorSystem actorSystem,
        ITenants tenants,
        CreateStreamProcessorActorProps createStreamProcessorActorProps,
        CancellationToken cancellationToken
    )
    {
        _logger = logger;
        _streamDefinitions = streamDefinitions;
        _arguments = arguments;
        _executionContext = executionContext;
        _actorSystem = actorSystem;
        _tenants = tenants;
        _createStreamProcessorActorProps = createStreamProcessorActorProps;
        _eventProcessorForTenant = eventProcessorForTenant;
        _acceptRegistration = acceptRegistration;
        _rejectRegistration = rejectRegistration;
        _metrics = metrics;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    StreamId TargetStream => _arguments.EventHandler.Value;

    /// <inheritdoc />
    public EventHandlerInfo Info => new(
        new EventHandlerId(_arguments.Scope, _arguments.EventHandler),
        _arguments.HasAlias,
        _arguments.Alias,
        _arguments.EventTypes,
        _arguments.Partitioned);

    public ScopeId Scope => _arguments.Scope.Value;

    public EventProcessorId EventProcessor => _arguments.EventHandler.Value;

    StreamProcessorId StreamProcessorId => new(Scope, EventProcessor, EventProcessor.Value);

    IEnumerable<ArtifactId> EventTypes => _arguments.EventTypes;

    bool Partitioned => _arguments.Partitioned;

    public StreamDefinition FilteredStreamDefinition => new(
        new TypeFilterWithEventSourcePartitionDefinition(
            TargetStream,
            TargetStream,
            EventTypes,
            Partitioned));

    public Task<Try<IDictionary<TenantId, IStreamProcessorState>>> GetEventHandlerCurrentState() =>
        RequestAsync<IDictionary<TenantId, IStreamProcessorState>, GetCurrentProcessorState>(GetCurrentProcessorState.Instance);

    public Task<Try<ProcessingPosition>> ReprocessEventsFrom(TenantId tenant, ProcessingPosition position)
        => RequestAsync<ProcessingPosition, ReprocessEventsFrom>(new ReprocessEventsFrom(tenant, position));

    public Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents() => Try<IDictionary<TenantId, Try<ProcessingPosition>>>.DoAsync(
        async () =>
        {
            var results = new Dictionary<TenantId, Try<ProcessingPosition>>();

            foreach (var tenantId in _tenants.All)
            {
                var result = await RequestAsync<ProcessingPosition, ReprocessEventsFrom>(new ReprocessEventsFrom(tenantId, ProcessingPosition.Initial));
                results.Add(tenantId, result);
            }

            return results;
        });


    Task<Try<TR>> RequestAsync<TR, TM>(TM message, CancellationToken cancellationToken = default) where TM : class
    {
        if (_eventHandlerKindPid == null)
            return Task.FromResult(Try<TR>.Failed(new ArgumentException("Event handler is in an invalid state")));
        return Try<TR>.DoAsync(
            async () =>
            {
                var response = await _actorSystem.Root.RequestAsync<object>(_eventHandlerKindPid,
                    message,
                    cancellationToken);
                return response switch
                {
                    TR state => state,
                    Exception exception => throw exception,
                    _ => throw new ArgumentException("Unexpected response from GetEventHandlerCurrentState: " + response)
                };
            });
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_eventHandlerKindPid != null)
            {
                _actorSystem.Root.StopAsync(_eventHandlerKindPid).GetAwaiter().GetResult();
            }

            // EventProcessorStreamProcessor?.Dispose();
            _cancellationTokenSource.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public async Task RegisterAndStart()
    {
        _logger.ConnectingEventHandlerWithId(EventProcessor);
        await Start().ConfigureAwait(false);
    }

    // /// <inheritdoc />
    // public event EventHandlerRegistrationFailed? OnRegistrationFailed;


    async Task Start()
    {
        void ProcessedEvent(TenantId tenant, StreamEvent @event, TimeSpan time)
        {
            _metrics.IncrementEventsProcessedTotal(Info, tenant, @event, time);
        }

        void FailedToProcessEvent(TenantId tenant, StreamEvent @event, TimeSpan time)
        {
            _metrics.IncrementEventsProcessedTotal(Info, tenant, @event, time);
            _metrics.IncrementEventProcessingFailuresTotal(Info, tenant, @event);
        }

        var props = _createStreamProcessorActorProps.Invoke(StreamProcessorId, FilteredStreamDefinition, _eventProcessorForTenant,
            ProcessedEvent, FailedToProcessEvent, _executionContext, _cancellationTokenSource);
        _eventHandlerKindPid = _actorSystem.Root.SpawnNamed(props, EventProcessor.Value.ToString());


        // _logger.StartingEventHandler(FilterDefinition.TargetStream);
        try
        {
            var runningDispatcher = _acceptRegistration(_cancellationTokenSource.Token);


            await PersistFilter().ConfigureAwait(false);
            var tasks = new List<Task>
            {
                runningDispatcher
            };
            var taskGroup = new TaskGroup(tasks);


            taskGroup.OnFirstTaskFailure += (_, ex) =>
            {
                if (ex is OperationCanceledException)
                {
                    _logger.CancelledRunningEventHandler(ex, EventProcessor, Scope);
                }
                else
                {
                    _logger.ErrorWhileRunningEventHandler(ex, EventProcessor, Scope);
                }
            };
            taskGroup.OnAllTasksCompleted += () => _logger.EventHandlerDisconnected(EventProcessor, Scope);

            await taskGroup.WaitForAllCancellingOnFirst(_cancellationTokenSource).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _logger.ErrorWhileStartingEventHandler(ex, EventProcessor, Scope);
            }

            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }

    async Task PersistFilter()
    {
        var filteredStreamDefinition = new StreamDefinition(FilteredStreamDefinition.FilterDefinition);
        _logger.PersistingStreamDefinition(filteredStreamDefinition.StreamId);
        await _streamDefinitions.Persist(Scope, filteredStreamDefinition, _cancellationTokenSource.Token).ConfigureAwait(false);
    }

    Task Fail(FailureId failureId, string message)
        => Fail(new Failure(failureId, message));

    Task Fail(Failure failure)
    {
        return _rejectRegistration(failure, _cancellationTokenSource.Token);
    }
}
