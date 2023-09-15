// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Legacy;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Proto;
using CommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public delegate Props CreateTenantScopedStreamProcessorProps(StreamProcessorId streamProcessorId,
    TypeFilterWithEventSourcePartitionDefinition filterDefinition,
    IEventProcessor processor,
    ExecutionContext executionContext,
    ScopedStreamProcessorProcessedEvent onProcessed,
    ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
    EventHandlerInfo eventHandlerInfo,
    TenantId tenantId);

/// <summary>
/// Represents the basis of system that can process a stream of events.
/// </summary>
public sealed class TenantScopedStreamProcessorActor : IActor, IDisposable
{
    readonly TenantId _tenantId;
    readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
    readonly IEventProcessor _processor;
    readonly IStreamEventSubscriber _eventSubscriber;
    readonly ExecutionContext _executionContext;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IMapStreamPositionToEventLogPosition _eventLogPositionEnricher;
    readonly ScopedStreamProcessorProcessedEvent _onProcessed;
    readonly ScopedStreamProcessorFailedToProcessEvent _onFailedToProcess;
    readonly IEventFetchers _eventFetchers;
    readonly IStreamProcessorLifecycleHooks _lifecycleHooks;
    readonly IFetchStartPosition _positionFetcher;
    readonly List<IDisposable> _cleanup = new();

    readonly bool _partitioned;
    readonly int _concurrency;

    static readonly TimeSpan _runtimeShutdownTimeout = TimeSpan.FromSeconds(30);
    readonly StartFrom _startFrom;
    readonly DateTimeOffset? _stopAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedStreamProcessorActor"/> class.
    /// </summary>
    /// <param name="eventHandlerInfo"></param>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="filterDefinition"></param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="streamProcessorStates"></param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="eventLogPositionEnricher"></param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    /// <param name="eventSubscriber"></param>
    /// <param name="onProcessed"></param>
    /// <param name="eventFetchers"></param>
    /// <param name="onFailedToProcess"></param>
    /// <param name="lifecycleHooks"></param>
    /// <param name="positionFetcher"></param>
    public TenantScopedStreamProcessorActor(
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        IStreamEventSubscriber eventSubscriber,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        IMapStreamPositionToEventLogPosition eventLogPositionEnricher,
        ILogger<TenantScopedStreamProcessorActor> logger,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        IEventFetchers eventFetchers,
        EventHandlerInfo eventHandlerInfo,
        TenantId tenantId,
        IStreamProcessorLifecycleHooks lifecycleHooks,
        IFetchStartPosition positionFetcher)
    {
        Identifier = streamProcessorId;
        Logger = logger;
        _onProcessed = onProcessed;
        _onFailedToProcess = onFailedToProcess;
        _tenantId = tenantId;
        _lifecycleHooks = lifecycleHooks;
        _positionFetcher = positionFetcher;
        _eventFetchers = eventFetchers;
        _eventLogPositionEnricher = eventLogPositionEnricher;
        _eventSubscriber = eventSubscriber;
        _streamProcessorStates = streamProcessorStates;
        _filterDefinition = filterDefinition;
        _processor = processor;
        _executionContext = executionContext;
        _partitioned = filterDefinition.Partitioned;
        _concurrency = eventHandlerInfo.Concurrency;
        _startFrom = eventHandlerInfo.StartFrom;
        _stopAt = eventHandlerInfo.StopAt;
    }

    public static CreateTenantScopedStreamProcessorProps CreateFactory(ICreateProps createProps)
        => (streamProcessorId, filterDefinition, processor, executionContext, onProcessed, onFailedToProcess, eventHandlerInfo, tenantId) =>
            PropsFor(createProps, streamProcessorId, filterDefinition, processor, executionContext, onProcessed, onFailedToProcess, eventHandlerInfo, tenantId);

    static Props PropsFor(ICreateProps createProps,
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        EventHandlerInfo eventHandlerInfo,
        TenantId tenantId
    )
    {
        return createProps.PropsFor<TenantScopedStreamProcessorActor>(
            streamProcessorId,
            filterDefinition,
            processor,
            executionContext,
            onProcessed,
            onFailedToProcess,
            eventHandlerInfo,
            tenantId);
    }

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            switch (context.Message)
            {
                case Started:
                    await Init(context);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug("Cancelled processing of {Message}", context.Message);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to process message {Message}", context.Message);
            throw;
        }
    }


    async Task Init(IContext context)
    {
        // Don't start before the startFrom timestamp, if present
        var initTimestamp = DateTimeOffset.UtcNow;
        if (_startFrom.SpecificTimestamp != null && _startFrom.SpecificTimestamp > initTimestamp)
        {
            Logger.DeferringStartOfStreamProcessor(Identifier, _startFrom.SpecificTimestamp.Value);
            var after = WaitForIt(_startFrom.SpecificTimestamp.Value, context.CancellationToken);
            context.ReenterAfter(after, _ =>
            {
                Logger.CompletedDeferredStartOfStreamProcessor(Identifier, DateTimeOffset.UtcNow - initTimestamp);
                return Init(context);
            });
            return;
        }

        var processingPosition = await LoadProcessingPosition(context);
        if (!processingPosition.Success)
        {
            Logger.FailedToLoadProcessingPosition(processingPosition.Exception, Identifier);
            throw processingPosition.Exception;
        }


        var initialState = processingPosition.Result;
        LogInitialState(initialState);

        var from = initialState.Position;

        var (shutdownToken, deadlineToken) = GetCancellationTokens(context);

        var events = _stopAt is not null ? SubscribeUntil(from, _stopAt.Value, shutdownToken) : StartSubscription(from, null, shutdownToken);

        context.ReenterAfter(Task.CompletedTask, _ => StartProcessing(initialState, events, context, shutdownToken, deadlineToken));
    }

    async Task WaitForIt(DateTimeOffset until, CancellationToken cancellationToken)
    {
        while (until > DateTimeOffset.UtcNow)
        {
            var delay = until - DateTimeOffset.UtcNow;
            if (delay.TotalMilliseconds > int.MaxValue)
            {
                await Task.Delay(int.MaxValue, cancellationToken);
            }
            else
            {
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    void LogInitialState(IStreamProcessorState initialState)
    {
        if (initialState.FailingPartitionCount > 0)
        {
            Logger.StartingStreamProcessorWithFailingPartitions(Identifier, initialState.Position, initialState.FailingPartitionCount,
                initialState.EarliestProcessingPosition);
        }
        else
        {
            Logger.StartingStreamProcessor(Identifier, initialState.Position);
        }
    }

    (CancellationToken shutdownToken, CancellationToken deadlineToken) GetCancellationTokens(IContext context)
    {
        var systemShutdownHook = _lifecycleHooks.RegisterShutdownHook();
        var shutdownAffectingTokens = new List<CancellationToken>
        {
            context.CancellationToken,
            systemShutdownHook.SystemStoppingToken
        };
        if (_processor.ShutdownToken is not null)
        {
            shutdownAffectingTokens.Add(_processor.ShutdownToken!.Value);
        }

        var shutdownTokenSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownAffectingTokens.ToArray());
        var deadlineTokenSource = _processor.DeadlineToken is not null
            ? CancellationTokenSource.CreateLinkedTokenSource(_processor.DeadlineToken!.Value)
            : new CancellationTokenSource();

        var cancellationTokenRegistration = shutdownTokenSource.Token.Register(() =>
        {
            if (_processor.ShutdownToken is null || !_processor.ShutdownToken.Value.IsCancellationRequested)
            {
                // If the source of the shutdown is not the processor itself, then we need to cancel the deadline token after a set delay
                deadlineTokenSource.CancelAfter(_runtimeShutdownTimeout);
            }
        });

        _cleanup.Add(systemShutdownHook); // Important to dispose this, otherwise the system will not shut down cleanly
        _cleanup.Add(shutdownTokenSource);
        _cleanup.Add(deadlineTokenSource);
        _cleanup.Add(cancellationTokenRegistration);

        return (shutdownTokenSource.Token, deadlineTokenSource.Token);
    }

    async Task StartProcessing(IStreamProcessorState streamProcessorState, ChannelReader<StreamEvent> events, IContext context, CancellationToken stoppingToken,
        CancellationToken deadlineToken)
    {
        try
        {
            if (_concurrency > 1 && _partitioned)
            {
                var streamDefinition = new StreamDefinition(new FilterDefinition(SourceStream: StreamId.EventLog, StreamId.EventLog, Partitioned: true));
                var fetcher = await _eventFetchers.GetFetcherFor(Identifier.ScopeId, streamDefinition, stoppingToken);

                var processor = new ConcurrentPartitionedProcessor(
                    Identifier,
                    _filterDefinition.Types,
                    _processor,
                    _streamProcessorStates,
                    _executionContext,
                    _onProcessed,
                    _onFailedToProcess,
                    _tenantId,
                    (ICanFetchEventsFromPartitionedStream)fetcher,
                    _concurrency,
                    Logger);

                await processor.Process(events, streamProcessorState, stoppingToken, deadlineToken);
            }
            else if (_partitioned)
            {
                var streamDefinition = new StreamDefinition(new FilterDefinition(SourceStream: StreamId.EventLog, StreamId.EventLog, Partitioned: true));
                var fetcher = await _eventFetchers.GetFetcherFor(Identifier.ScopeId, streamDefinition, stoppingToken);

                var processor = new PartitionedProcessor(
                    Identifier,
                    _filterDefinition.Types,
                    _processor,
                    _streamProcessorStates,
                    _executionContext,
                    _onProcessed,
                    _onFailedToProcess,
                    _tenantId,
                    (ICanFetchEventsFromPartitionedStream)fetcher,
                    Logger);

                await processor.Process(events, streamProcessorState, stoppingToken, deadlineToken);
            }
            else
            {
                var processor = new NonPartitionedProcessor(
                    Identifier,
                    _filterDefinition,
                    _processor,
                    _streamProcessorStates,
                    _executionContext,
                    _onProcessed,
                    _onFailedToProcess,
                    _tenantId,
                    Logger);

                await processor.Process(events, streamProcessorState, stoppingToken, deadlineToken);
            }

            Logger.EventHandlerDisconnectedForTenant(Identifier.EventProcessorId, Identifier.ScopeId, _tenantId);
        }
        catch (OperationCanceledException e)
        {
            Logger.CancelledRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
        catch (Exception e)
        {
            Logger.ErrorWhileRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
        finally
        {
            // ReSharper disable once MethodHasAsyncOverload
            context.Stop(context.Self);
        }
    }

    ChannelReader<StreamEvent> SubscribeUntil(ProcessingPosition from, DateTimeOffset to, CancellationToken token)
    {
        var unixTimeSeconds = to.ToUnixTimeSeconds();
        return StartSubscription(from, evt => evt.Occurred.Seconds >= unixTimeSeconds, token);
    }


    ChannelReader<StreamEvent> StartSubscription(ProcessingPosition from, Predicate<CommittedEvent>? until, CancellationToken token)
    {
        return _eventSubscriber.Subscribe(
            Identifier.ScopeId,
            _filterDefinition.Types.ToList(),
            from,
            _filterDefinition.Partitioned,
            $"sp:{Identifier.EventProcessorId}",
            until,
            token);
    }


    /// <summary>
    /// Loads processing position from storage, optionally enriching it with the event log position.
    /// If no processing position is found, it will return a new <see cref="Streams.StreamProcessorState"/> with the initial position.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    async Task<Try<IStreamProcessorState>> LoadProcessingPosition(IContext context)
    {
        var position = await _streamProcessorStates.TryGetFor(Identifier, context.CancellationToken);
        if (position is { Success: false, Exception: StreamProcessorStateDoesNotExist })
        {
            return Try<IStreamProcessorState>.Succeeded(await GetInitialProcessorState(context.CancellationToken));
        }

        var positionWithEventLog = await position.ReduceAsync(WithEventLogPosition);


        if (_startFrom.SpecificTimestamp is not null)
        {
            // Double check that we are at or after the startFrom timestamp.
            // If the startFrom parameter was changed from before, we might need to skip the in-between events.
            var minimumStartPosition = await _positionFetcher.GetInitialProcessorSequence(Identifier.ScopeId, _startFrom, context.CancellationToken);
            if (minimumStartPosition > EventLogSequenceNumber.Initial)
            {
                return positionWithEventLog.Select<IStreamProcessorState>(pos =>
                {
                    if (pos.EarliestProcessingPosition.EventLogPosition > minimumStartPosition)
                    {
                        // Already later than the minimum start position, no need to change anything
                        return pos;
                    }

                    return pos.SkipEventsBefore(minimumStartPosition);
                });
            }
        }

        return positionWithEventLog;

        Task<Try<IStreamProcessorState>> WithEventLogPosition(IStreamProcessorState state)
        {
            return _eventLogPositionEnricher
                .WithEventLogSequence(new StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>(Identifier, state), context.CancellationToken);
        }
    }

    async ValueTask<IStreamProcessorState> GetInitialProcessorState(CancellationToken cancellationToken)
    {
        var eventLogSequenceNumber = await _positionFetcher.GetInitialProcessorSequence(Identifier.ScopeId, _startFrom, cancellationToken);
        var processingPosition = new ProcessingPosition(StreamPosition.Start, eventLogSequenceNumber);

        return _partitioned
            ? new Streams.Partitioned.StreamProcessorState(processingPosition, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty,
                DateTimeOffset.Now)
            : new StreamProcessorState(processingPosition, DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="TenantScopedStreamProcessorActor"/>.
    /// </summary>
    public StreamProcessorId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }

    public void Dispose()
    {
        foreach (var disposable in _cleanup)
        {
            disposable.Dispose();
        }

        _cleanup.Clear();
    }
}
