// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Legacy;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Actors;

record GetCurrentState
{
    static readonly GetCurrentState Instance = new();
};

/// <summary>
/// Represents the basis of system that can process a stream of events.
/// </summary>
public class StreamProcessorActor : IActor
{
    static readonly TimeSpan _eventWaiterTimeout = TimeSpan.FromMinutes(1);
    
    readonly IStreamDefinition _sourceStreamDefinition;
    readonly TenantId _tenantId;
    readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
    readonly IEventProcessor _processor;
    readonly IStreamEventSubscriber _eventSubscriber;
    readonly ExecutionContext _executionContext;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IMapStreamPositionToEventLogPosition _eventLogPositionEnricher;
    readonly ICanGetTimeToRetryFor<StreamProcessorState> _timeToRetryGetter;


    IStreamProcessorState _currentState;
    bool _started;
    ProcessingPosition _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorActor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="filterDefinition"></param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="streamProcessorStates"></param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    /// <param name="eventSubscriber"></param>
    protected StreamProcessorActor(
        // TenantId tenantId,
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        IStreamEventSubscriber eventSubscriber,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        IMapStreamPositionToEventLogPosition eventLogPositionEnricher,
        ILogger logger, ICanGetTimeToRetryFor<StreamProcessorState> timeToRetryGetter)
    {
        Identifier = streamProcessorId;
        Logger = logger;
        _timeToRetryGetter = timeToRetryGetter;
        _eventLogPositionEnricher = eventLogPositionEnricher;
        _eventSubscriber = eventSubscriber;
        _streamProcessorStates = streamProcessorStates;
        // _tenantId = tenantId;
        _filterDefinition = filterDefinition;
        _processor = processor;
        _executionContext = executionContext;
    }


    public Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Started:
                return Init(context);
            case StreamEvent evt:
                return OnStreamEvent(evt, context);
            case GetCurrentState:
                return OnGetCurrentState(context);
            case Stopping:
                return OnStopping(context);
        }


        return Task.CompletedTask;
    }

    Task OnStopping(IContext context)
    {
        throw new NotImplementedException();
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

    }

    void StartSubscription(IContext context, ProcessingPosition from, CancellationToken token)
    {


        _ = Task.Run(async () =>
        {
            var events = _eventSubscriber.Subscribe(Identifier.ScopeId, _filterDefinition.Types, from, _filterDefinition.Partitioned,
                token);
            
            await foreach (var evt in events.WithCancellation(token))
            {
                await context.RequestAsync<Ack>(context.Self, evt);
            }
        }, token);

    }

    async Task OnStreamEvent(StreamEvent evt, IContext ctx)
    {
        ctx.Respond(Ack.Instance);
        var result = await _processor.Process(evt.Event, evt.Partition, GetExecutionContextForEvent(evt), ctx.CancellationToken);
        
        
        
    }

    /// <summary>
    /// Loads processing position from storage, optionally enriching it with the event log position.
    /// If no processing position is found, it will return a new <see cref="StreamProcessorState"/> with the initial position.
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
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="StreamProcessorActor"/>.
    /// </summary>
    public StreamProcessorId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="IStreamProcessorState"/>.
    /// </summary>
    /// <returns>Current <see cref="IStreamProcessorState"/>.</returns>
    /// <summary>
    /// Event that occurs when the Scoped Stream Processor has successfully processed an event.
    /// </summary>
    public ScopedStreamProcessorProcessedEvent OnProcessedEvent;

    /// <summary>
    /// Event that occurs when the Scoped Stream Processor failed to processed an event.
    /// </summary>
    public ScopedStreamProcessorFailedToProcessEvent OnFailedToProcessedEvent;

    /// <summary>
    /// Starts the stream processing.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Start(CancellationToken cancellationToken)
    {
        if (_started)
        {
            throw new StreamProcessorAlreadyProcessingStream(Identifier);
        }

        _started = true;
        return BeginProcessing(cancellationToken);
    }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="events">The <see cref="StreamEvent" /> events to process..</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected virtual async Task<IStreamProcessorState> ProcessEvents(IEnumerable<(StreamEvent, ExecutionContext)> events, IStreamProcessorState currentState,
        CancellationToken cancellationToken)
    {
        foreach (var (@event, executionContext) in events)
        {
            (currentState, var processingResult) = await ProcessEvent(@event, currentState, executionContext, cancellationToken).ConfigureAwait(false);
            if (!processingResult.Succeeded)
            {
                break;
            }
        }

        return currentState;
    }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="event">The <see cref="StreamEvent" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to use for processing the event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected async Task<(IStreamProcessorState, IProcessingResult)> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState,
        ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var processingResult = await _processor.Process(@event.Event, @event.Partition, executionContext, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        return (await HandleProcessingResult(processingResult, @event, stopwatch.Elapsed, currentState).ConfigureAwait(false), processingResult);
    }

    /// <summary>
    /// Gets the <see cref="TimeSpan" /> for when to retry processing again.
    /// </summary>
    /// <param name="state">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>The time to retry <see cref="TimeSpan" />.</returns>
    protected TimeSpan GetTimeToRetryProcessing(IStreamProcessorState state)
    {
        var result = _eventWaiterTimeout;

        if (TryGetTimeToRetry(state, out var timeToRetry))
        {
            if (timeToRetry < result)
            {
                result = timeToRetry;
            }
        }

        return result;
    }

    protected bool TryGetTimeToRetry(IStreamProcessorState state, out TimeSpan timeToRetry)
        => _timeToRetryGetter.TryGetTimespanToRetry(state as StreamProcessorState, out timeToRetry);
    
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
    protected async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts,
        IStreamProcessorState currentState, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var processingResult = await _processor
            .Process(@event.Event, @event.Partition, failureReason, processingAttempts - 1, executionContext, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        return await HandleProcessingResult(processingResult, @event, stopwatch.Elapsed, currentState).ConfigureAwait(false);
    }

    /// <summary>
    /// Handle the <see cref="IProcessingResult" /> from the processing of a <see cref="StreamEvent" />..
    /// </summary>
    /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The processed <see cref="StreamEvent" />.</param>
    /// <param name="processingTime">The time it took to process the event.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent, TimeSpan processingTime,
        IStreamProcessorState currentState)
    {
        if (processingResult.Retry)
        {
            OnFailedToProcessedEvent?.Invoke(processedEvent, processingTime);
            return OnRetryProcessingResult(processingResult as FailedProcessing, processedEvent, currentState);
        }

        if (!processingResult.Succeeded)
        {
            OnFailedToProcessedEvent?.Invoke(processedEvent, processingTime);
            return OnFailedProcessingResult(processingResult as FailedProcessing, processedEvent, currentState);
        }

        OnProcessedEvent?.Invoke(processedEvent, processingTime);
        return OnSuccessfulProcessingResult(processingResult as SuccessfulProcessing, processedEvent, currentState);
    }

    /// <inheritdoc/>
     Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState) =>
        _failingPartitions.AddFailingPartitionFor(
            Identifier,
            currentState as StreamProcessorState,
            processedEvent.Position,
            processedEvent.Partition,
            DateTimeOffset.MaxValue,
            failedProcessing.FailureReason,
            CancellationToken.None);

    /// <inheritdoc/>
     Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState) =>
        _failingPartitions.AddFailingPartitionFor(
            Identifier,
            currentState as StreamProcessorState,
            processedEvent.Position,
            processedEvent.Partition,
            DateTimeOffset.UtcNow.Add(failedProcessing.RetryTimeout),
            failedProcessing.FailureReason,
            CancellationToken.None);
    
    /// <inheritdoc/>
     async Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState)
    {
        var oldState = currentState as Partitioned.StreamProcessorState;
        var newState = new Partitioned.StreamProcessorState(processedEvent.NextProcessingPosition, oldState.FailingPartitions,
            DateTimeOffset.UtcNow);
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
        return newState;
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

    async Task BeginProcessing(CancellationToken cancellationToken)
    {
        try
        {
            do
            {
                Try<IEnumerable<StreamEvent>> tryGetEvents;
                do
                {
                    _resetStreamProcessor = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    if (_resetStreamProcessorCompletionSource != default)
                    {
                        if (_newPosition > _currentState.Position.StreamPosition)
                        
                            _resetStreamProcessorCompletionSource.SetResult(
                                Try<StreamPosition>.Failed(
                                    new CannotSetStreamProcessorPositionHigherThanCurrentPosition(Identifier, _currentState, _newPosition)));
                        }
                        else
                        {
                            Log.ScopedStreamProcessorPerformingSetToPositionAction(Logger, Identifier);
                            var result = await _resetStreamProcessorAction(_tenantId, cancellationToken).ConfigureAwait(false);
                            if (result.Success)
                            {
                                Log.ScopedStreamProcessorSetToPosition(Logger, Identifier, _newPosition);
                                _currentState = await SetNewStateWithPosition(_currentState, _newPosition).ConfigureAwait(false);
                                _resetStreamProcessorCompletionSource.SetResult(_currentState.Position.StreamPosition);
                            }
                            else
                            {
                                Log.ScopedStreamProcessorPerformingSetToPositionActionFailed(Logger, Identifier, result.Exception);
                                _resetStreamProcessorCompletionSource.SetResult(Try<StreamPosition>.Failed(result.Exception));
                            }
                        }

                        _resetStreamProcessorCompletionSource = default;
                        _resetStreamProcessorAction = default;
                    }

                    try
                    {
                        _currentState = await Catchup(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
                        tryGetEvents = await FetchNextEventsToProcess(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
                        if (!tryGetEvents.Success)
                        {
                            await _eventWaiter.WaitForEvent(
                                Identifier.ScopeId,
                                _sourceStreamDefinition.StreamId,
                                _currentState.Position.StreamPosition,
                                GetTimeToRetryProcessing(_currentState),
                                _resetStreamProcessor.Token).ConfigureAwait(false);
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        tryGetEvents = ex;
                    }
                    finally
                    {
                        _resetStreamProcessor?.Dispose();
                        _resetStreamProcessor = null;
                    }
                } while (!tryGetEvents.Success && !cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var eventsToProcess = tryGetEvents.Result;
                _currentState = await ProcessEvents(eventsToProcess.Select(_ => (_, GetExecutionContextForEvent(_))), _currentState, cancellationToken)
                    .ConfigureAwait(false);
            } while (!cancellationToken.IsCancellationRequested);
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Log.StreamProcessingForTenantFailed(Logger, ex, Identifier, _tenantId);
            }
        }
        finally
        {
            _resetStreamProcessor?.Dispose();
            _resetStreamProcessor = null;
        }
    }

class Ack
{
    Ack()
    { }
    public static readonly Ack Instance = new();
}
