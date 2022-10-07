// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents a system for working with all the <see cref="AbstractScopedStreamProcessor" /> registered for <see cref="ITenants.All" />.
/// </summary>
public class StreamProcessor : IDisposable
{
    readonly StreamProcessorId _identifier;
    readonly EventProcessorKind _eventProcessorKind;
    readonly IStreamDefinition _streamDefinition;
    readonly IPerformActionsForAllTenants _forAllTenants;
    readonly Func<TenantId, IEventProcessor> _createEventProcessorFor;
    readonly Func<TenantId, ICreateScopedStreamProcessors> _getCreateScopedStreamProcessors;
    readonly Action _unregister;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    readonly ExecutionContext _executionContext;
    readonly CancellationTokenSource _stopAllScopedStreamProcessorsTokenSource;
    readonly Dictionary<TenantId, AbstractScopedStreamProcessor> _streamProcessors = new();
    bool _initialized;
    bool _started;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The identifier of the stream processor.</param>
    /// <param name="eventProcessorKind ">The kind of the event processor.</param>
    /// <param name="streamDefinition">The definition of the stream the processor should process events from.</param>
    /// <param name="forAllTenants">The performer to use to create scoped stream processors for all tenants.</param>
    /// <param name="createEventProcessorFor">The factory to use to create an event processor per tenant.</param>
    /// <param name="getCreateScopedStreamProcessors">The factory to us to get the scoped stream processor creator per tenant.</param>
    /// <param name="unregister">The callback to call to unregister the stream processor when it completes or fails.</param>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="executionContext">The execution context to run the processor in.</param>
    /// <param name="cancellationToken">The cancellation token that is cancelled when the stream processor should stop processing.</param>
    public StreamProcessor(
        StreamProcessorId streamProcessorId,
        EventProcessorKind eventProcessorKind,
        IStreamDefinition streamDefinition,
        IPerformActionsForAllTenants forAllTenants,
        Action unregister,
        Func<TenantId, IEventProcessor> createEventProcessorFor,
        Func<TenantId, ICreateScopedStreamProcessors> getCreateScopedStreamProcessors,
        IMetricsCollector metrics,
        ILogger logger,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        _identifier = streamProcessorId;
        _eventProcessorKind = eventProcessorKind;
        _forAllTenants = forAllTenants;
        _streamDefinition = streamDefinition;
        _createEventProcessorFor = createEventProcessorFor;
        _getCreateScopedStreamProcessors = getCreateScopedStreamProcessors;
        _metrics = metrics;
        _unregister = unregister;
        _logger = logger;
        _executionContext = executionContext;
        _stopAllScopedStreamProcessorsTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    /// <summary>
    /// Event that occurs when the Stream Processor has successfully processed an event.
    /// </summary>
    public StreamProcessorProcessedEvent OnProcessedEvent;

    /// <summary>
    /// Event that occurs when the Stream Processor failed to processed an event.
    /// </summary>
    public StreamProcessorFailedToProcessEvent OnFailedToProcessedEvent;

    /// <summary>
    /// Gets all current <see cref="IStreamProcessorState"/> states. 
    /// </summary>
    /// <returns>The <see cref="IStreamProcessorState"/> per <see cref="TenantId"/>.</returns>
    public Try<IDictionary<TenantId, IStreamProcessorState>> GetCurrentStates()
        => _initialized
            ? _streamProcessors.ToDictionary(_ => _.Key, _ => _.Value.GetCurrentState())
            : new StreamProcessorNotInitialized(_identifier);

    /// <summary>
    /// Initializes the stream processor.
    /// </summary>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    public async Task Initialize()
    {
        Log.InitializingStreamProcessor(_logger, _identifier);
        _metrics.IncrementInitializations(_eventProcessorKind);

        _stopAllScopedStreamProcessorsTokenSource.Token.ThrowIfCancellationRequested();
        if (_initialized)
        {
            throw new StreamProcessorAlreadyInitialized(_identifier);
        }
        _initialized = true;

        await _forAllTenants.PerformAsync(async (tenant, _) =>
        {
            var eventProcessor = _createEventProcessorFor(tenant);
            var scopedStreamProcessorsCreator = _getCreateScopedStreamProcessors(tenant);
            
            var scopedStreamProcessor = await scopedStreamProcessorsCreator.Create(
                _streamDefinition,
                _identifier,
                eventProcessor,
                _executionContext,
                _stopAllScopedStreamProcessorsTokenSource.Token).ConfigureAwait(false);
            
            scopedStreamProcessor.OnProcessedEvent += (@event, time) =>
            {
                _metrics.IncrementEventsProcessed(_eventProcessorKind, time);
                OnProcessedEvent?.Invoke(tenant, @event, time);
            };
            scopedStreamProcessor.OnFailedToProcessedEvent += (@event, time) =>
            {
                _metrics.IncrementEventsProcessed(_eventProcessorKind, time);
                _metrics.IncrementFailedEventsProcessed(_eventProcessorKind);
                OnFailedToProcessedEvent?.Invoke(tenant, @event, time);
            };
            // TODO: Do we need to remove these on disposal maybe?
            
            _streamProcessors.Add(tenant, scopedStreamProcessor);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the stream processing for all tenants.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Start()
    {
        Log.StartingStreamProcessor(_logger, _identifier);
        _metrics.IncrementStarts(_eventProcessorKind);

        if (!_initialized)
        {
            throw new StreamProcessorNotInitialized(_identifier);
        }

        if (_started)
        {
            throw new StreamProcessorAlreadyProcessingStream(_identifier);
        }

        _started = true;
        try
        {
            var tasks = new TaskGroup(StartScopedStreamProcessors(_stopAllScopedStreamProcessorsTokenSource.Token));
            
            tasks.OnFirstTaskFailure += (_, ex) =>
            {
                Log.ScopedStreamProcessorFailed(_logger, ex, _identifier);
                _metrics.IncrementFailures(_eventProcessorKind);
            };
            await tasks.WaitForAllCancellingOnFirst(_stopAllScopedStreamProcessorsTokenSource).ConfigureAwait(false);
        }
        finally
        {
            _unregister();
        }
    }

    /// <summary>
    /// Sets the position of the stream processor for a tenant.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    public Task<Try<StreamPosition>> SetToPosition(TenantId tenant, StreamPosition position)
        => PerformActionAndSetToPosition(tenant, position, (_, _) => Task.FromResult(Try.Succeeded()));

    /// <summary>
    /// Sets the position of the stream processors for all tenant to be the initial <see cref="StreamPosition"/>.
    /// </summary>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    public Task<IDictionary<TenantId, Try<StreamPosition>>> SetToInitialPositionForAllTenants()
        => PerformActionAndSetToInitialPositionForAllTenants((_, _) => Task.FromResult(Try.Succeeded()));

    /// <summary>
    /// Performs an action, then sets the position of the stream processor for a tenant.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <param name="action">The action to perform before setting the position.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    public Task<Try<StreamPosition>> PerformActionAndSetToPosition(TenantId tenant, StreamPosition position, Func<TenantId, CancellationToken, Task<Try>> action)
    {
        if (!_streamProcessors.TryGetValue(tenant, out var streamProcessor))
        {
            return Task.FromResult<Try<StreamPosition>>(new StreamProcessorNotRegisteredForTenant(_identifier, tenant));
        }

        _metrics.IncrementPositionSet(_eventProcessorKind);
        return streamProcessor.PerformActionAndReprocessEventsFrom(position, action);
    }

    /// <summary>
    /// Performs an action, then sets the position of the stream processors for all tenant to be the initial <see cref="StreamPosition"/>.
    /// </summary>
    /// <param name="action">The action to perform before setting the position.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    public async Task<IDictionary<TenantId, Try<StreamPosition>>> PerformActionAndSetToInitialPositionForAllTenants(Func<TenantId, CancellationToken, Task<Try>> action)
    {
        _metrics.IncrementInitialPositionSetForAllTenants(_eventProcessorKind);

        var tasks = _streamProcessors
            .ToDictionary(_ => _.Key, _ => _.Value.PerformActionAndReprocessEventsFrom(StreamPosition.Start, action));

        var result = new Dictionary<TenantId, Try<StreamPosition>>();

        foreach (var (tenant, task) in tasks)
        {
            result.Add(tenant, await task.ConfigureAwait(false));
        }

        return result;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the object.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed state.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _stopAllScopedStreamProcessorsTokenSource.Cancel();
            _stopAllScopedStreamProcessorsTokenSource.Dispose();

            if (!_started)
            {
                _unregister();
            }
        }

        _disposed = true;
    }

    IEnumerable<Task> StartScopedStreamProcessors(CancellationToken cancellationToken) => _streamProcessors.Select(
        _ => Task.Run(async () =>
        {
            var (_, streamProcessor) = _;
            await streamProcessor.Start(cancellationToken).ConfigureAwait(false);
        }, cancellationToken)).ToList();
}
