// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Legacy;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public delegate Props CreateTenantScopedStreamProcessorProps(StreamProcessorId streamProcessorId,
    TypeFilterWithEventSourcePartitionDefinition filterDefinition,
    IEventProcessor processor,
    ExecutionContext executionContext,
    ScopedStreamProcessorProcessedEvent onProcessed,
    ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
    TenantId tenantId);

/// <summary>
/// Represents the basis of system that can process a stream of events.
/// </summary>
public sealed class TenantScopedStreamProcessorActor : IActor, IDisposable
{
    readonly IStreamDefinition _sourceStreamDefinition;
    readonly TenantId _tenantId;
    readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
    readonly IEventProcessor _processor;
    readonly IStreamEventSubscriber _eventSubscriber;
    readonly ExecutionContext _executionContext;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IMapStreamPositionToEventLogPosition _eventLogPositionEnricher;
    readonly ScopedStreamProcessorProcessedEvent _onProcessed;
    readonly ScopedStreamProcessorFailedToProcessEvent _onFailedToProcess;
    readonly IHandleFailures _failureHandler;

    CancellationTokenSource _stoppingToken;
    bool retrying;

    IStreamProcessorState _currentState;

    bool _started;

    ProcessingPosition _current;
    readonly bool _partitioned;
    readonly FailedPartitionsRetrier _failingPartitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedStreamProcessorActor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="filterDefinition"></param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="streamProcessorStates"></param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    /// <param name="eventSubscriber"></param>
    /// <param name="onProcessed"></param>
    /// <param name="onFailedToProcess"></param>
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
        TenantId tenantId)
    {
        Identifier = streamProcessorId;
        Logger = logger;
        _onProcessed = onProcessed;
        _onFailedToProcess = onFailedToProcess;
        _tenantId = tenantId;
        _eventLogPositionEnricher = eventLogPositionEnricher;
        _eventSubscriber = eventSubscriber;
        _streamProcessorStates = streamProcessorStates;
        _filterDefinition = filterDefinition;
        _processor = processor;
        _executionContext = executionContext;
        _partitioned = filterDefinition.Partitioned;
        if (_partitioned)
        {
            _failingPartitions = new FailedPartitionsRetrier(_streamProcessorStates, processor, GetExecutionContextForEvent);
        }
    }

    public static CreateTenantScopedStreamProcessorProps CreateFactory(ICreateProps createProps)
        => (streamProcessorId, filterDefinition, processor, executionContext, onProcessed, onFailedToProcess, tenantId) =>
            PropsFor(createProps, streamProcessorId, filterDefinition, processor, executionContext, onProcessed, onFailedToProcess, tenantId);

    static Props PropsFor(ICreateProps createProps,
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId
    )
    {
        return createProps.PropsFor<TenantScopedStreamProcessorActor>(streamProcessorId, filterDefinition, processor, executionContext, onProcessed,
            onFailedToProcess, tenantId);
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
                case StreamEvent evt:
                    await OnStreamEvent(evt, context);
                    break;
                case GetCurrentProcessorState:
                    await OnGetCurrentState(context);
                    break;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to process message {Message}", context.Message);
            throw;
        }
    }


    Task OnGetCurrentState(IContext context)
    {
        context.Respond(_currentState);
        return Task.CompletedTask;
    }

    async Task Init(IContext context)
    {
        var processingPosition = await LoadProcessingPosition(context);
        if (!processingPosition.Success)
        {
            Logger.LogError(processingPosition.Exception, "Failed to load processing position for {StreamProcessorId}", Identifier);
            throw processingPosition.Exception;
        }

        _currentState = processingPosition.Result;

        var from = _currentState.EarliestPosition;
        var cts = new CancellationTokenSource();
        StartSubscription(context, from, cts.Token);
        _stoppingToken = cts;
    }

    void StartSubscription(IContext context, ProcessingPosition from, CancellationToken token)
    {
        _ = Task.Run(async () =>
        {
            var events = _eventSubscriber.Subscribe(Identifier.ScopeId, _filterDefinition.Types, from, _filterDefinition.Partitioned,
                token);

            await foreach (var evt in events.ReadAllAsync(token))
            {
                await context.RequestAsync<Ack>(context.Self, evt);
            }
        }, token);
    }

    async Task OnStreamEvent(StreamEvent evt, IContext ctx)
    {
        if (_partitioned && _currentState is Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState state &&
            state.FailingPartitions.ContainsKey(evt.Partition))
        {
            ctx.Respond(Ack.Instance);
            return;
        }

        var (streamProcessorState, processingResult) = await ProcessEvent(evt, _currentState, GetExecutionContextForEvent(evt), ctx.CancellationToken);

        if (processingResult.Succeeded)
        {
            ctx.Respond(Ack.Instance);
            return;
        }

        HandleFailure(evt, streamProcessorState, processingResult, ctx);
    }

    void HandleFailure(StreamEvent evt, IStreamProcessorState streamProcessorState, IProcessingResult processingResult, IContext ctx)
    {
        if (_partitioned)
        {
            HandlePartitionedFailure(evt, processingResult, ctx);
        }
        else
        {
            HandleNonPartitionedFailure(evt, processingResult, ctx);
        }
    }

    void HandlePartitionedFailure(StreamEvent evt, IProcessingResult processingResult, IContext ctx)
    {
        _failingPartitions.WriteToFailedPartition(evt);
        PerformPartitionedRetriesAfterDelay(ctx);
        ctx.Respond(Ack.Instance);
    }

    void PerformPartitionedRetriesAfterDelay(IContext ctx)
    {
        if (!retrying && _failingPartitions.HasFailedEvents && _currentState.TryGetTimespanToRetry(out var retryTimeout))
        {
            ctx.ReenterAfter(Task.Delay(retryTimeout, _stoppingToken.Token), async task =>
            {
                if (_currentState is not Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState streamProcessorState)
                {
                    Logger.LogError("Current state is not a partitioned state");
                    return;
                }

                _currentState = await _failingPartitions.CatchupFor(Identifier,
                    streamProcessorState,
                    _stoppingToken.Token);
                retrying = false;
                PerformPartitionedRetriesAfterDelay(ctx);
            });
        }
    }


    void HandleNonPartitionedFailure(StreamEvent evt, IProcessingResult processingResult, IContext ctx)
    {
        if (_currentState is not StreamProcessorState streamProcessorState)
        {
            Logger.LogError("Current state is a partitioned state");
            throw new ArgumentException("Current state is a partitioned state");
        }

        var capturedContext = ctx.Capture();
        if (_currentState.TryGetTimespanToRetry(out var retryTimeout))
        {
            ctx.ReenterAfter(Task.Delay(retryTimeout, _stoppingToken.Token), async task =>
            {
                ctx.Apply(capturedContext);
                _currentState = await RetryProcessingEvent(evt, processingResult.FailureReason, streamProcessorState.ProcessingAttempts + 1,
                    GetExecutionContextForEvent(evt), ctx.CancellationToken);
                if (processingResult.Succeeded)
                {
                    ctx.Respond(Ack.Instance);
                    return;
                }

                HandleNonPartitionedFailure(evt, processingResult, ctx);
            });
        }
        else
        {
            ctx.Respond(Ack.Instance);
        }
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
        if (!position.Success && position.Exception is StreamProcessorStateDoesNotExist)
        {
            return new StreamProcessorState(ProcessingPosition.Initial, DateTimeOffset.UtcNow);
        }

        return await position.ReduceAsync(WithEventLogPosition);

        Task<Try<IStreamProcessorState>> WithEventLogPosition(IStreamProcessorState state) => _eventLogPositionEnricher
            .WithEventLogSequence(new StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>(Identifier, state), context.CancellationToken);
    }


    /// <summary>
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="TenantScopedStreamProcessorActor"/>.
    /// </summary>
    public StreamProcessorId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }


    // /// <summary>
    // /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    // /// </summary>
    // /// <param name="events">The <see cref="StreamEvent" /> events to process..</param>
    // /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    // /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    // /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    // protected async Task<IStreamProcessorState> ProcessEvents(IEnumerable<(StreamEvent, ExecutionContext)> events, IStreamProcessorState currentState,
    //     CancellationToken cancellationToken)
    // {
    //     foreach (var (@event, executionContext) in events)
    //     {
    //         (currentState, var processingResult) = await ProcessEvent(@event, currentState, executionContext, cancellationToken).ConfigureAwait(false);
    //         if (!processingResult.Succeeded)
    //         {
    //             break;
    //         }
    //     }
    //
    //     return currentState;
    // }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="event">The <see cref="StreamEvent" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to use for processing the event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    async Task<(IStreamProcessorState, IProcessingResult)> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState,
        ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var before = Stopwatch.GetTimestamp();
        var processingResult = await _processor.Process(@event.Event, @event.Partition, executionContext, cancellationToken).ConfigureAwait(false);
        var elapsed = Stopwatch.GetElapsedTime(before);
        return (await HandleProcessingResult(processingResult, @event, elapsed, currentState).ConfigureAwait(false), processingResult);
    }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="event">The <see cref="StreamEvent" />.</param>
    /// <param name="failureReason">The reason for why processing failed the last time.</param>
    /// <param name="processingAttempts">The number of times that this event has been processed before.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to use for processing the event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts, ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var processingResult = await _processor
            .Process(@event.Event, @event.Partition, failureReason, processingAttempts - 1, executionContext, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        return await HandleProcessingResult(processingResult, @event, stopwatch.Elapsed, _currentState).ConfigureAwait(false);
    }

    /// <summary>
    /// Handle the <see cref="IProcessingResult" /> from the processing of a <see cref="StreamEvent" />..
    /// </summary>
    /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The processed <see cref="StreamEvent" />.</param>
    /// <param name="processingTime">The time it took to process the event.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    async Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent, TimeSpan processingTime,
        IStreamProcessorState currentState)
    {
        var newState = currentState.WithResult(processingResult, processedEvent, DateTimeOffset.UtcNow);
        OnProcessingResult(processingResult, processedEvent, processingTime);
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
        return newState;
    }

    void OnProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent, TimeSpan processingTime)
    {
        if (processingResult.Succeeded)
        {
            _onProcessed.Invoke(processedEvent, processingTime);
        }
        else
        {
            _onFailedToProcess.Invoke(processedEvent, processingTime);
        }
    }

    /// <summary>
    /// Gets the <see cref="ExecutionContext"/> to use while processing the specified <see cref="StreamEvent"/>.
    /// </summary>
    /// <param name="eventToProcess">The event to create the processing execution context for.</param>
    /// <returns>The <see cref="ExecutionContext"/> to use while processing the event.</returns>
    protected ExecutionContext GetExecutionContextForEvent(StreamEvent eventToProcess)
        => _executionContext with
        {
            Tenant = _tenantId,
            CorrelationId = eventToProcess.Event.ExecutionContext.CorrelationId,
            // TODO: Does this make sense? Do we want more, or less?
        };

    // async Task BeginProcessing(CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         do
    //         {
    //             Try<IEnumerable<StreamEvent>> tryGetEvents;
    //             do
    //             {
    //                 _resetStreamProcessor = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    //                 if (_resetStreamProcessorCompletionSource != default)
    //                 {
    //                     if (_newPosition > _currentState.Position.StreamPosition)
    //                     
    //                         _resetStreamProcessorCompletionSource.SetResult(
    //                             Try<StreamPosition>.Failed(
    //                                 new CannotSetStreamProcessorPositionHigherThanCurrentPosition(Identifier, _currentState, _newPosition)));
    //                     }
    //                     else
    //                     {
    //                         Log.ScopedStreamProcessorPerformingSetToPositionAction(Logger, Identifier);
    //                         var result = await _resetStreamProcessorAction(_tenantId, cancellationToken).ConfigureAwait(false);
    //                         if (result.Success)
    //                         {
    //                             Log.ScopedStreamProcessorSetToPosition(Logger, Identifier, _newPosition);
    //                             _currentState = await SetNewStateWithPosition(_currentState, _newPosition).ConfigureAwait(false);
    //                             _resetStreamProcessorCompletionSource.SetResult(_currentState.Position.StreamPosition);
    //                         }
    //                         else
    //                         {
    //                             Log.ScopedStreamProcessorPerformingSetToPositionActionFailed(Logger, Identifier, result.Exception);
    //                             _resetStreamProcessorCompletionSource.SetResult(Try<StreamPosition>.Failed(result.Exception));
    //                         }
    //                     }
    //
    //                     _resetStreamProcessorCompletionSource = default;
    //                     _resetStreamProcessorAction = default;
    //                 }
    //
    //                 try
    //                 {
    //                     _currentState = await Catchup(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
    //                     tryGetEvents = await FetchNextEventsToProcess(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
    //                     if (!tryGetEvents.Success)
    //                     {
    //                         await _eventWaiter.WaitForEvent(
    //                             Identifier.ScopeId,
    //                             _sourceStreamDefinition.StreamId,
    //                             _currentState.Position.StreamPosition,
    //                             GetTimeToRetryProcessing(_currentState),
    //                             _resetStreamProcessor.Token).ConfigureAwait(false);
    //                     }
    //                 }
    //                 catch (TaskCanceledException ex)
    //                 {
    //                     tryGetEvents = ex;
    //                 }
    //                 finally
    //                 {
    //                     _resetStreamProcessor?.Dispose();
    //                     _resetStreamProcessor = null;
    //                 }
    //             } while (!tryGetEvents.Success && !cancellationToken.IsCancellationRequested);
    //
    //             if (cancellationToken.IsCancellationRequested)
    //             {
    //                 break;
    //             }
    //
    //             var eventsToProcess = tryGetEvents.Result;
    //             _currentState = await ProcessEvents(eventsToProcess.Select(_ => (_, GetExecutionContextForEvent(_))), _currentState, cancellationToken)
    //                 .ConfigureAwait(false);
    //         } while (!cancellationToken.IsCancellationRequested);
    //     }
    //     catch (Exception ex)
    //     {
    //         if (!cancellationToken.IsCancellationRequested)
    //         {
    //             Log.StreamProcessingForTenantFailed(Logger, ex, Identifier, _tenantId);
    //         }
    //     }
    //     finally
    //     {
    //         _resetStreamProcessor?.Dispose();
    //         _resetStreamProcessor = null;
    //     }
    // }

    class Ack
    {
        Ack()
        {
        }

        public static Ack Instance { get; } = new();
    }

    public void Dispose()
    {
        _stoppingToken.Cancel();
        _stoppingToken.Dispose();
    }
}
