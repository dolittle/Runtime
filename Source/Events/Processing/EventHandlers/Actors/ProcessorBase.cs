// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

/// <summary>
/// Represents the basis of system that can process a stream of events.
/// </summary>
public abstract class ProcessorBase
{
    readonly TenantId _tenantId;
    readonly IEventProcessor _processor;
    readonly ExecutionContext _executionContext;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly ScopedStreamProcessorProcessedEvent _onProcessed;
    readonly ScopedStreamProcessorFailedToProcessEvent _onFailedToProcess;

    bool waitingForEvent;

    CancellationTokenSource? _stoppingToken;
    bool retrying;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedStreamProcessorActor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="streamProcessorStates"></param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="eventLogPositionEnricher"></param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    /// <param name="eventSubscriber"></param>
    /// <param name="onProcessed"></param>
    /// <param name="onFailedToProcess"></param>
    public ProcessorBase(
        StreamProcessorId streamProcessorId,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ILogger logger)
    {
        Identifier = streamProcessorId;
        _onProcessed = onProcessed;
        _onFailedToProcess = onFailedToProcess;
        _tenantId = tenantId;
        Logger = logger;
        _streamProcessorStates = streamProcessorStates;
        _processor = processor;
        _executionContext = executionContext;
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

    /// <summary>
    /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="TenantScopedStreamProcessorActor"/>.
    /// </summary>
    public StreamProcessorId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="evt">The <see cref="StreamEvent" />.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to use for processing the event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected async Task<(IStreamProcessorState, IProcessingResult)> ProcessEvent(StreamEvent evt, IStreamProcessorState currentState,
        ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Processing event at position {ProcessingPosition}", evt.CurrentProcessingPosition);

        var before = Stopwatch.GetTimestamp();
        var processingResult = await _processor.Process(evt.Event, evt.Partition, executionContext, cancellationToken).ConfigureAwait(false);
        var elapsed = Stopwatch.GetElapsedTime(before);
        return (await HandleProcessingResult(processingResult, evt, elapsed, currentState).ConfigureAwait(false), processingResult);
    }

    /// <summary>
    /// Process the <see cref="StreamEvent" /> and get the new <see cref="IStreamProcessorState" />.
    /// </summary>
    /// <param name="evt">The <see cref="StreamEvent" />.</param>
    /// <param name="failureReason">The reason for why processing failed the last time.</param>
    /// <param name="processingAttempts">The number of times that this event has been processed before.</param>
    /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to use for processing the event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="IStreamProcessorState" />.</returns>
    protected async Task<(IStreamProcessorState, IProcessingResult)> RetryProcessingEvent(StreamEvent evt, IStreamProcessorState currentState,
        string failureReason,
        uint processingAttempts, ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("ReProcessing event at position {ProcessingPosition}", evt.CurrentProcessingPosition);
        var start = Stopwatch.GetTimestamp();
        var processingResult = await _processor.Process(
            evt.Event, evt.Partition, failureReason, processingAttempts - 1, executionContext, cancellationToken).ConfigureAwait(false);
        var elapsed = Stopwatch.GetElapsedTime(start);
        var updatedState = await HandleProcessingResult(processingResult, evt, elapsed, currentState).ConfigureAwait(false);
        return (updatedState, processingResult);
    }

    protected Task PersistNewState(IStreamProcessorId streamProcessorId, IStreamProcessorState newState, CancellationToken cancellationToken)
    {
        return _streamProcessorStates.Persist(streamProcessorId, newState, cancellationToken);
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
        Logger.LogInformation("Processing result for event at position {ProcessingPosition} is {ProcessingResult}", processedEvent.CurrentProcessingPosition,
            processingResult.GetType().Name);
        var newState = currentState.WithResult(processingResult, processedEvent, DateTimeOffset.UtcNow);
        OnProcessingResult(processingResult, processedEvent, processingTime);
        await PersistNewState(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
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
        };
}
