// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;

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
    readonly IAsyncPolicyFor<ICanFetchEventsFromStream> _fetchEventToProcessPolicy;
    readonly IStreamEventWatcher _eventWaiter;
    readonly object _setPositionLock = new();
    CancellationTokenSource _resetStreamProcessor;
    TaskCompletionSource<Try<StreamPosition>> _resetStreamProcessorCompletionSource;
    IStreamProcessorState _currentState;
    bool _started;
    StreamPosition _newPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractScopedStreamProcessor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="sourceStreamDefinition">The source stream <see cref="IStreamDefinition" />.</param>
    /// <param name="initialState">The initial state of the <see cref="IStreamProcessorState" />.</param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="eventsFetcher">The <see cref="ICanFetchEventsFromStream" />.</param>
    /// <param name="fetchEventsToProcessPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" /> to wait for events to be available in stream.</param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    protected AbstractScopedStreamProcessor(
        TenantId tenantId,
        IStreamProcessorId streamProcessorId,
        IStreamDefinition sourceStreamDefinition,
        IStreamProcessorState initialState,
        IEventProcessor processor,
        ICanFetchEventsFromStream eventsFetcher,
        IAsyncPolicyFor<ICanFetchEventsFromStream> fetchEventsToProcessPolicy,
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
        _fetchEventToProcessPolicy = fetchEventsToProcessPolicy;
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
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with a <see cref="StreamPosition"/>.</returns>
    public Task<Try<StreamPosition>> ReprocessEventsFrom(StreamPosition position)
    {
        try
        {
            if (_resetStreamProcessorCompletionSource != default)
            {
                return Task.FromResult<Try<StreamPosition>>(new AlreadySettingNewStreamProcessorPosition(Identifier));
            }
            var tcs = new TaskCompletionSource<Try<StreamPosition>>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_setPositionLock)
            {
                if (_resetStreamProcessorCompletionSource != default)
                {
                    return Task.FromResult<Try<StreamPosition>>(new AlreadySettingNewStreamProcessorPosition(Identifier));
                }
                _newPosition = position;
                _resetStreamProcessorCompletionSource = tcs;
            }
            
            _resetStreamProcessor?.Cancel();
            return tcs.Task;
        }
        catch (Exception ex)
        {
            return Task.FromResult<Try<StreamPosition>>(ex);
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
    /// Tries to get the <see cref="TimeSpan" /> for when to retry processing. Outputs the maximum value <see cref="TimeSpan" /> if there is no retry time.
    /// </summary>
    /// <param name="state">The current <see cref="IStreamProcessorState" />..</param>
    /// <param name="timeToRetry">The <see cref="TimeSpan" /> for when to retry processsing a stream processor.</param>
    /// <returns>A value indicating whether there is a retry time.</returns>
    protected abstract bool TryGetTimeToRetry(IStreamProcessorState state, out TimeSpan timeToRetry);

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
    /// <param name="event">The <see cref="StreamEvent" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected virtual async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState, CancellationToken cancellationToken)
    {
        var processingResult = await _processor.Process(@event.Event, @event.Partition, cancellationToken).ConfigureAwait(false);
        return await HandleProcessingResult(processingResult, @event, currentState).ConfigureAwait(false);
    }

    /// <summary>
    /// Fetches the Event that is should be processed next.
    /// </summary>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamEvent" />.</returns>
    protected Task<Try<StreamEvent>> FetchNextEventToProcess(IStreamProcessorState currentState, CancellationToken cancellationToken) =>
        _fetchEventToProcessPolicy.Execute(cancellationToken => _eventsFetcher.Fetch(currentState.Position, cancellationToken), cancellationToken);

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
            result = new[] { result, timeToRetry }.Min();
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
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts, IStreamProcessorState currentState, CancellationToken cancellationToken)
    {
        var processingResult = await _processor.Process(@event.Event, @event.Partition, failureReason, processingAttempts - 1, cancellationToken).ConfigureAwait(false);
        return await HandleProcessingResult(processingResult, @event, currentState).ConfigureAwait(false);
    }

    /// <summary>
    /// Handle the <see cref="IProcessingResult" /> from the processing of a <see cref="StreamEvent" />..
    /// </summary>
    /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
    /// <param name="processedEvent">The processed <see cref="StreamEvent" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent, IStreamProcessorState currentState)
    {
        if (processingResult.Retry)
        {
            return OnRetryProcessingResult(processingResult as FailedProcessing, processedEvent, currentState);
        }
        else if (!processingResult.Succeeded)
        {
            return OnFailedProcessingResult(processingResult as FailedProcessing, processedEvent, currentState);
        }

        return OnSuccessfulProcessingResult(processingResult as SuccessfulProcessing, processedEvent, currentState);
    }

    async Task BeginProcessing(CancellationToken cancellationToken)
    {
        try
        {
            do
            {
                Try<StreamEvent> tryGetEvent;
                do
                {
                    _resetStreamProcessor = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    if (_resetStreamProcessorCompletionSource != default)
                    {
                        if (_newPosition > _currentState.Position)
                        {
                            _resetStreamProcessorCompletionSource.SetResult(Try<StreamPosition>.Failed(new CannotSetStreamProcessorPositionHigherThanCurrentPosition(Identifier, _currentState, _newPosition)));
                        }
                        else
                        {
                            Log.ScopedStreamProcessorSetToPosition(Logger, Identifier, _newPosition);
                            _currentState = await SetNewStateWithPosition(_currentState, _newPosition).ConfigureAwait(false);
                            _resetStreamProcessorCompletionSource.SetResult(_currentState.Position);
                        }
                        _resetStreamProcessorCompletionSource = default;
                    }
                    try
                    {
                        _currentState = await Catchup(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
                        tryGetEvent = await FetchNextEventToProcess(_currentState, _resetStreamProcessor.Token).ConfigureAwait(false);
                        if (!tryGetEvent.Success)
                        {
                            await _eventWaiter.WaitForEvent(
                                Identifier.ScopeId,
                                _sourceStreamDefinition.StreamId,
                                _currentState.Position,
                                GetTimeToRetryProcessing(_currentState),
                                _resetStreamProcessor.Token).ConfigureAwait(false);
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        tryGetEvent = ex;
                    }
                }
                while (!tryGetEvent.Success && !cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                _currentState = await ProcessEvent(tryGetEvent, _currentState, cancellationToken).ConfigureAwait(false);
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
        }
    }
}
