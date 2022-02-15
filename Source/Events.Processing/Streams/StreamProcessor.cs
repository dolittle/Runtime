// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents a system for working with all the <see cref="AbstractScopedStreamProcessor" /> registered for <see cref="ITenants.All" />.
/// </summary>
public class StreamProcessor : IDisposable
{
    readonly Dictionary<TenantId, AbstractScopedStreamProcessor> _streamProcessors = new();
    readonly StreamProcessorId _identifier;
    readonly IPerformActionOnAllTenants _onAllTenants;
    readonly IStreamDefinition _streamDefinition;
    readonly Func<IEventProcessor> _getEventProcessor;
    readonly Action _unregister;
    readonly Func<ICreateScopedStreamProcessors> _getScopedStreamProcessorsCreator;
    readonly IExecutionContextManager _executionContextManager;
    readonly ILogger<StreamProcessor> _logger;
    readonly CancellationTokenSource _stopAllScopedStreamProcessorsTokenSource;
    bool _initialized;
    bool _started;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
    /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
    /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
    /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns an <see cref="IEventProcessor" />.</param>
    /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
    /// <param name="getScopedStreamProcessorsCreator">The <see cref="ICreateScopedStreamProcessors" />.</param>
    /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    public StreamProcessor(
        StreamProcessorId streamProcessorId,
        IPerformActionOnAllTenants onAllTenants,
        IStreamDefinition streamDefinition,
        Func<IEventProcessor> getEventProcessor,
        Action unregister,
        Func<ICreateScopedStreamProcessors> getScopedStreamProcessorsCreator,
        IExecutionContextManager executionContextManager,
        ILogger<StreamProcessor> logger,
        CancellationToken cancellationToken)
    {
        _identifier = streamProcessorId;
        _onAllTenants = onAllTenants;
        _streamDefinition = streamDefinition;
        _getEventProcessor = getEventProcessor;
        _unregister = unregister;
        _getScopedStreamProcessorsCreator = getScopedStreamProcessorsCreator;
        _executionContextManager = executionContextManager;
        _logger = logger;
        _stopAllScopedStreamProcessorsTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

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

        _stopAllScopedStreamProcessorsTokenSource.Token.ThrowIfCancellationRequested();
        if (_initialized)
        {
            throw new StreamProcessorAlreadyInitialized(_identifier);
        }
        _initialized = true;

        await _onAllTenants.PerformAsync(async tenant =>
        {
            var scopedStreamProcessorsCreators = _getScopedStreamProcessorsCreator();
            var scopedStreamProcessor = await scopedStreamProcessorsCreators.Create(
                _streamDefinition,
                _identifier,
                _getEventProcessor(),
                _stopAllScopedStreamProcessorsTokenSource.Token).ConfigureAwait(false);
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
            
            tasks.OnFirstTaskFailure += (_, ex) => Log.ScopedStreamProcessorFailed(_logger, ex, _identifier);

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
        => _streamProcessors.TryGetValue(tenant, out var streamProcessor)
            ? streamProcessor.PerformActionAndReprocessEventsFrom(position, action)
            : Task.FromResult<Try<StreamPosition>>(new StreamProcessorNotRegisteredForTenant(_identifier, tenant));
    
    /// <summary>
    /// Performs an action, then sets the position of the stream processors for all tenant to be the initial <see cref="StreamPosition"/>.
    /// </summary>
    /// <param name="action">The action to perform before setting the position.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    public async Task<IDictionary<TenantId, Try<StreamPosition>>> PerformActionAndSetToInitialPositionForAllTenants(Func<TenantId, CancellationToken, Task<Try>> action)
    {
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
            var (tenant, streamProcessor) = _;
            _executionContextManager.CurrentFor(tenant);
            await streamProcessor.Start(cancellationToken).ConfigureAwait(false);
        }, cancellationToken)).ToList();
}
