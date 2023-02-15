// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public delegate Props CreateStreamProcessorActorProps(
    StreamProcessorId streamProcessorId,
    IStreamDefinition streamDefinition,
    Func<TenantId, IEventProcessor> createEventProcessorFor,
    StreamProcessorProcessedEvent processedEvent,
    StreamProcessorFailedToProcessEvent failedToProcessEvent,
    ExecutionContext executionContext,
    CancellationTokenSource cancellationTokenSource);

/// <summary>
/// Represents a system for working with all the <see cref="TenantScopedStreamProcessorActor" /> registered for <see cref="ITenants.All" />.
/// </summary>
public class StreamProcessorActor : IDisposable, IActor
{
    readonly StreamProcessorId _identifier;
    readonly IStreamDefinition _streamDefinition;
    readonly ITenants _tenants;
    readonly Func<TenantId, IEventProcessor> _createEventProcessorFor;
    readonly Streams.IMetricsCollector _metrics;
    readonly ILogger<StreamProcessorActor> _logger;
    readonly Func<TenantId, CreateTenantScopedStreamProcessorProps> _getCreateScopedStreamProcessorProps;
    readonly StreamProcessorProcessedEvent _onProcessedEvent;
    readonly StreamProcessorFailedToProcessEvent _onFailedToProcessEvent;
    readonly ExecutionContext _executionContext;
    readonly CancellationTokenSource _stopAllProcessors;
    readonly TypeFilterWithEventSourcePartitionDefinition _filter;
    readonly Dictionary<TenantId, PID> _streamProcessors = new();
    readonly CancellationTokenSource _stopEverything;
    bool _started;
    bool _disposed;


    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorActor"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The identifier of the stream processor.</param>
    /// <param name="streamDefinition">The definition of the stream the processor should process events from.</param>
    /// <param name="createEventProcessorFor">The factory to use to create an event processor per tenant.</param>
    /// <param name="getCreateScopedStreamProcessorProps">The factory to us to get the scoped stream processor creator per tenant.</param>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="executionContext">The execution context to run the processor in.</param>
    /// <param name="stoppingToken">The cancellation token that is cancelled when the stream processor should stop processing.</param>
    public StreamProcessorActor(
        StreamProcessorId streamProcessorId,
        IStreamDefinition streamDefinition,
        Func<TenantId, IEventProcessor> createEventProcessorFor,
        ExecutionContext executionContext,
        Streams.IMetricsCollector metrics,
        ITenants tenants,
        StreamProcessorProcessedEvent onProcessedEvent,
        StreamProcessorFailedToProcessEvent onFailedToProcessEvent,
        ILogger<StreamProcessorActor> logger,
        Func<TenantId, CreateTenantScopedStreamProcessorProps> getCreateScopedStreamProcessorProps,
        CancellationTokenSource stoppingToken)
    {
        if (streamDefinition.FilterDefinition is not TypeFilterWithEventSourcePartitionDefinition filter)
        {
            throw new ArgumentException("streamDefinition.FilterDefinition definition must be of type TypeFilterWithEventSourcePartitionDefinition",
                nameof(streamDefinition));
        }

        _filter = filter;
        _identifier = streamProcessorId;
        _streamDefinition = streamDefinition;
        _tenants = tenants;
        _createEventProcessorFor = createEventProcessorFor;
        _metrics = metrics;
        _logger = logger;
        _getCreateScopedStreamProcessorProps = getCreateScopedStreamProcessorProps;
        _onProcessedEvent = onProcessedEvent;
        _onFailedToProcessEvent = onFailedToProcessEvent;
        _executionContext = executionContext;
        _stopAllProcessors = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken.Token);
        _stopEverything = stoppingToken;
    }

    public static CreateStreamProcessorActorProps CreateFactory(ICreateProps provider)
        => (streamProcessorId, streamDefinition, createEventProcessorFor, processedEvent, failedToProcessEvent, executionContext, cancellationTokenSource) =>
            CreatePropsFor(provider, streamProcessorId, streamDefinition, createEventProcessorFor, processedEvent, failedToProcessEvent, executionContext,
                cancellationTokenSource);

    static Props CreatePropsFor(ICreateProps provider,
        StreamProcessorId streamProcessorId,
        IStreamDefinition streamDefinition,
        Func<TenantId, IEventProcessor> createEventProcessorFor,
        StreamProcessorProcessedEvent processedEvent,
        StreamProcessorFailedToProcessEvent failedToProcessEvent,
        ExecutionContext executionContext,
        CancellationTokenSource cancellationTokenSource)
        => provider.PropsFor<StreamProcessorActor>(
            streamProcessorId, streamDefinition, createEventProcessorFor, executionContext, processedEvent, failedToProcessEvent, cancellationTokenSource);

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    await OnStarted(context);
                    break;
                case Stopping:
                    await OnStopping(context);
                    break;
                case GetCurrentProcessorState:
                    await OnGetCurrentProcessorState(context);
                    break;
                case ReprocessEventsFrom reprocess:
                    await OnReprocessEventsFrom(reprocess, context);
                    break;
                case Terminated terminated:
                    await OnTerminated(terminated, context);
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while processing message {Message}", context.Message);
            throw;
        }
    }

    async Task OnReprocessEventsFrom(ReprocessEventsFrom reprocess, IContext context)
    {
        try
        {
            if (_streamProcessors.Remove(reprocess.TenantId, out var pid))
            {
                await context.StopAsync(pid);
                InitTenant(reprocess.TenantId, context);
            }
            else
            {
                context.Respond(new ArgumentException("TenantId not found"));
            }
        }
        catch (Exception e)
        {
            context.Respond(e);
        }
    }

    async Task OnGetCurrentProcessorState(IContext context)
    {
        var states = new Dictionary<TenantId, IStreamProcessorState>();
        try
        {
            foreach (var (tenantId, pid) in _streamProcessors)
            {
                var state = await context.RequestAsync<IStreamProcessorState>(pid, GetCurrentProcessorState.Instance);
                states.Add(tenantId, state);
            }

            context.Respond(states);
        }
        catch (Exception exception)
        {
            context.Respond(exception);
        }
    }

    Task OnTerminated(Terminated terminated, IContext context)
    {
        var stoppedChild = _streamProcessors.FirstOrDefault(_ => _.Value.Equals(terminated.Who));
        if (stoppedChild.Key is not null)
        {
            if (_stopEverything.IsCancellationRequested)
            {
                _logger.ErrorWhileRunningEventHandler(default, _identifier.EventProcessorId, _identifier.ScopeId);
                _stopEverything.Cancel();
            }

            _streamProcessors.Remove(stoppedChild.Key);
        }

        if (_streamProcessors.Count == 0)
        {
            context.Stop(context.Self);
        }

        return Task.CompletedTask;
    }

    async Task OnStopping(IContext context)
    {
        // Log.StoppingStreamProcessor(_logger, _identifier);
        _stopAllProcessors.Cancel();
        await Task.WhenAll(context.Children.Select(context.StopAsync));
    }

    async Task OnStarted(IContext context)
    {
        Streams.Log.InitializingStreamProcessor(_logger, _identifier);

        if (_stopAllProcessors.Token.IsCancellationRequested)
        {
            // ReSharper disable once MethodHasAsyncOverload
            context.Stop(context.Self);
            return;
        }

        _metrics.IncrementInitializations(EventProcessorKind.Actor);

        foreach (var tenantId in _tenants.All)
        {
            InitTenant(tenantId, context);
        }
    }

    void InitTenant(TenantId tenant, IContext context)
    {
        void OnProcessed(StreamEvent @event, TimeSpan time)
        {
            _metrics.IncrementEventsProcessed(EventProcessorKind.Actor, time);
            _onProcessedEvent?.Invoke(tenant, @event, time);
        }

        void FailedToProcessedEvent(StreamEvent @event, TimeSpan time)
        {
            _metrics.IncrementEventsProcessed(EventProcessorKind.Actor, time);
            _metrics.IncrementFailedEventsProcessed(EventProcessorKind.Actor);
            _onFailedToProcessEvent?.Invoke(tenant, @event, time);
        }

        var props = _getCreateScopedStreamProcessorProps(tenant).Invoke(_identifier, _filter, _createEventProcessorFor(tenant), _executionContext,
            OnProcessed, FailedToProcessedEvent, tenant);

        var tenantProcessorPid = context.Spawn(props);

        _streamProcessors.Add(tenant, tenantProcessorPid);
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
            _stopAllProcessors.Cancel();
            _stopAllProcessors.Dispose();
        }

        _disposed = true;
    }
}
