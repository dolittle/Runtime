// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents the basis of system that can process a stream of events.
/// </summary>
public abstract class AbstractScopedStreamProcessor
{
    static readonly TimeSpan _eventWaiterTimeout = TimeSpan.FromMinutes(1);
    readonly IStreamDefinition _sourceStreamDefinition;
    readonly TenantId _tenantId;
    readonly IEventProcessor _processor;
    readonly ICanFetchEventsFromStream _eventsFetcher;
    readonly ExecutionContext _executionContext;
    readonly IEventFetcherPolicies _eventFetcherPolicies;
    readonly IStreamEventWatcher _eventWaiter;
    readonly object _setPositionLock = new();
    CancellationTokenSource? _resetStreamProcessor;
    TaskCompletionSource<Try<ProcessingPosition>>? _resetStreamProcessorCompletionSource;
    Func<TenantId, CancellationToken, Task<Try>> _resetStreamProcessorAction;
    IStreamProcessorState _currentState;
    bool _started;
    ProcessingPosition _newPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractScopedStreamProcessor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="sourceStreamDefinition">The source stream <see cref="IStreamDefinition" />.</param>
    /// <param name="initialState">The initial state of the <see cref="IStreamProcessorState" />.</param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="eventsFetcher">The <see cref="ICanFetchEventsFromStream" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="eventFetcherPolicies">The policies to use while fetching events.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" /> to wait for events to be available in stream.</param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    protected AbstractScopedStreamProcessor(
        TenantId tenantId,
        IStreamProcessorId streamProcessorId,
        IStreamDefinition sourceStreamDefinition,
        IStreamProcessorState initialState,
        IEventProcessor processor,
        ICanFetchEventsFromStream eventsFetcher,
        ExecutionContext executionContext,
        IEventFetcherPolicies eventFetcherPolicies,
        IStreamEventWatcher streamWatcher,
        ILogger logger)
    {
        Identifier = streamProcessorId;
        Logger = logger;
        _currentState = initialState;
        _sourceStreamDefinition = sourceStreamDefinition;
        _tenantId = tenantId;
        _processor = processor;
        _eventsFetcher = eventsFetcher;
        _executionContext = executionContext;
        _eventFetcherPolicies = eventFetcherPolicies;
        _eventWaiter = streamWatcher;
    }

    /// <summary>
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="AbstractScopedStreamProcessor"/>.
    /// </summary>
    public IStreamProcessorId Identifier { get; }
        
    /// <summary>
    /// Gets the <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }
        
    /// <summary>
    /// Gets the current <see cref="IStreamProcessorState"/>.
    /// </summary>
    /// <returns>Current <see cref="IStreamProcessorState"/>.</returns>
    public IStreamProcessorState GetCurrentState() => _currentState;

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
    /// Sets the <see cref="IStreamProcessorState" /> to be at the given position.
    /// </summary>
    /// <remarks>
    /// This method fails with a <see cref="CannotSetStreamProcessorPositionHigherThanCurrentPosition"/> if trying to reprocess events
    /// from a <see cref="StreamPosition"/> that is higher than the current <see cref="StreamPosition"/>.
    /// </remarks>
    /// <param name="position">The <see cref="StreamPosition"/> to start processing events from.</param>
    /// <param name="action">The action to perform before setting the position.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with a <see cref="StreamPosition"/>.</returns>
    public Task<Try<ProcessingPosition>> PerformActionAndReprocessEventsFrom(ProcessingPosition position, Func<TenantId, CancellationToken, Task<Try>> action)
    {
        try
        {
            if (_resetStreamProcessorCompletionSource != default)
            {
                return Task.FromResult<Try<ProcessingPosition>>(new AlreadySettingNewStreamProcessorPosition(Identifier));
            }
            var tcs = new TaskCompletionSource<Try<ProcessingPosition>>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_setPositionLock)
            {
                if (_resetStreamProcessorCompletionSource != default)
                {
                    return Task.FromResult<Try<ProcessingPosition>>(new AlreadySettingNewStreamProcessorPosition(Identifier));
                }
                _newPosition = position;
                _resetStreamProcessorAction = action;
                _resetStreamProcessorCompletionSource = tcs;
            }
            
            _resetStreamProcessor?.Cancel();
            return tcs.Task;
        }
        catch (Exception ex)
        {
            return Task.FromResult<Try<ProcessingPosition>>(ex);
        }
    }

    /// <summary>
    /// Catchup on failing Events.
    /// </summary>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IStreamProcessorState" />.</returns>
    protected abstract Task<IStreamProcessorState> Catchup(IStreamProcessorState currentState, CancellationToken cancellationToken);

    /// <summary>
    ///  Gets the new <see cref="IStreamProcessorState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the processing of the <see cref="StreamEvent" /> succeeded.
    /// </summary>
    /// <param name="successfulProcessing">The <see cref="SuccessfulProcessing" /> <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected abstract Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

    /// <summary>
    ///  Gets the new <see cref="IStreamProcessorState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should be processed again.
    /// </summary>
    /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
    /// /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected abstract Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

    /// <summary>
    ///  Gets the new <see cref="IStreamProcessorState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should not be processed again.
    /// </summary>
    /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected abstract Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

    /// <summary>
    /// Creates a new <see cref="IStreamProcessorState"/> with the given <see cref="StreamPosition" />.
    /// </summary>
    /// <param name="currentState">The current <see cref="IStreamProcessorState"/>.</param>
    /// <param name="position">The new <see cref="StreamPosition"/>.</param>
    /// <returns>The new <see cref="IStreamProcessorState"/>.</returns>
    protected abstract Task<IStreamProcessorState> SetNewStateWithPosition(IStreamProcessorState currentState, StreamPosition position);

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="events">The <see cref="StreamEvent" /> events to process..</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected virtual async Task<IStreamProcessorState> ProcessEvents(IEnumerable<(StreamEvent, ExecutionContext)> events, IStreamProcessorState currentState, CancellationToken cancellationToken)
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
    protected async Task<(IStreamProcessorState, IProcessingResult)> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var processingResult = await _processor.Process(@event.Event, @event.Partition, @event.Position, executionContext, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        return (await HandleProcessingResult(processingResult, @event, stopwatch.Elapsed, currentState).ConfigureAwait(false), processingResult);
    }

    /// <summary>
    /// Fetches the Event that is should be processed next.
    /// </summary>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamEvent" />.</returns>
    protected Task<Try<IEnumerable<StreamEvent>>> FetchNextEventsToProcess(IStreamProcessorState currentState, CancellationToken cancellationToken)
        => _eventFetcherPolicies.Fetching.ExecuteAsync(_ => _eventsFetcher.Fetch(currentState.Position.StreamPosition, _), cancellationToken);
    // TODO: Shouldn't this policy rather be used in the actual fetcher?

    /// <summary>
    /// Gets the <see cref="TimeSpan" /> for when to retry processing again.
    /// </summary>
    /// <param name="state">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>The time to retry <see cref="TimeSpan" />.</returns>
    protected TimeSpan GetTimeToRetryProcessing(IStreamProcessorState state)
    {
        var result = _eventWaiterTimeout;
        
        if (state.TryGetTimespanToRetry(out var timeToRetry))
        {
            if (timeToRetry < result)
            {
                result = timeToRetry;
            }
        }

        return result;
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
    protected async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts, IStreamProcessorState currentState, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var processingResult = await _processor.Process(@event.Event, @event.Partition, @event.Position, failureReason, processingAttempts - 1, executionContext, cancellationToken).ConfigureAwait(false);
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
    protected Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent, TimeSpan processingTime, IStreamProcessorState currentState)
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
                        if (_newPosition.StreamPosition > _currentState.Position.StreamPosition)
                        {
                            _resetStreamProcessorCompletionSource.SetResult(Try<ProcessingPosition>.Failed(new CannotSetStreamProcessorPositionHigherThanCurrentPosition(Identifier, _currentState, _newPosition)));
                        }
                        else
                        {
                            Log.ScopedStreamProcessorPerformingSetToPositionAction(Logger, Identifier);
                            var result = await _resetStreamProcessorAction(_tenantId, cancellationToken).ConfigureAwait(false);
                            if (result.Success)
                            {
                                Log.ScopedStreamProcessorSetToPosition(Logger, Identifier, _newPosition.StreamPosition);
                                _currentState = await SetNewStateWithPosition(_currentState, _newPosition.StreamPosition).ConfigureAwait(false);
                                _resetStreamProcessorCompletionSource.SetResult(_currentState.Position);
                            }
                            else
                            {
                                Log.ScopedStreamProcessorPerformingSetToPositionActionFailed(Logger, Identifier, result.Exception);
                                _resetStreamProcessorCompletionSource.SetResult(Try<ProcessingPosition>.Failed(result.Exception));
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
                }
                while (!tryGetEvents.Success && !cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var eventsToProcess = tryGetEvents.Result;
                _currentState = await ProcessEvents(eventsToProcess.Select(_ => (_, GetExecutionContextForEvent(_))), _currentState, cancellationToken).ConfigureAwait(false);
            }
            while (!cancellationToken.IsCancellationRequested);
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
}
