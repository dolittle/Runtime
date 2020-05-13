// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents the basis of system that can process a stream of events.
    /// </summary>
    public abstract class AbstractScopedStreamProcessor
    {
        readonly TenantId _tenantId;
        readonly IEventProcessor _processor;
        IStreamProcessorState _currentState;
        bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractScopedStreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        /// <param name="initialState">The initial state of the <see cref="IStreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        protected AbstractScopedStreamProcessor(
            TenantId tenantId,
            IStreamProcessorId streamProcessorId,
            IStreamProcessorState initialState,
            IEventProcessor processor,
            ILogger logger)
        {
            Identifier = streamProcessorId;
            Logger = logger;
            _currentState = initialState;
            _tenantId = tenantId;
            _processor = processor;
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
        /// Starts the stream processing.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Start(CancellationToken cancellationToken)
        {
            if (_started) throw new StreamProcessorAlreadyProcessingStream(Identifier);
            _started = true;
            return BeginProcessing(cancellationToken);
        }

        /// <summary>
        /// Catchup on failing Events.
        /// </summary>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IStreamProcessorState" />.</returns>
        protected abstract Task<IStreamProcessorState> Catchup(IStreamProcessorState currentState, CancellationToken cancellationToken);

        /// <summary>
        /// Fetches the Event that is should be processed next.
        /// </summary>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamEvent" />.</returns>
        protected abstract Task<StreamEvent> FetchEventToProcess(IStreamProcessorState currentState, CancellationToken cancellationToken);

        /// <summary>
        ///  Gets the new <see cref="_currentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the processing of the <see cref="StreamEvent" /> succeeded.
        /// </summary>
        /// <param name="successfulProcessing">The <see cref="SuccessfulProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="_currentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

        /// <summary>
        ///  Gets the new <see cref="_currentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="_currentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

        /// <summary>
        ///  Gets the new <see cref="_currentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should not be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="_currentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState);

        /// <summary>
        /// Process the <see cref="StreamEvent" /> and get the new <see cref="_currentState" />.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent" />.</param>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="_currentState" />.</returns>
        protected virtual async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState, CancellationToken cancellationToken)
        {
            var processingResult = await _processor.Process(@event.Event, @event.Partition, cancellationToken).ConfigureAwait(false);
            return await HandleProcessingResult(processingResult, @event, currentState).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the <see cref="StreamEvent" /> and get the new <see cref="_currentState" />.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent" />.</param>
        /// <param name="failureReason">The reason for why processing failed the last time.</param>
        /// <param name="processingAttempts">The number of times that this event has been processed before.</param>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="_currentState" />.</returns>
        protected async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts, IStreamProcessorState currentState, CancellationToken cancellationToken)
        {
            var processingResult = await _processor.Process(@event.Event, @event.Partition, failureReason, processingAttempts - 1, cancellationToken).ConfigureAwait(false);
            return await HandleProcessingResult(processingResult, @event, currentState).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle the <see cref="IProcessingResult" /> from the procssing of a <see cref="StreamEvent" />..
        /// </summary>
        /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The processed <see cref="StreamEvent" />.</param>
        /// <param name="currentState">The current <see cref="IStreamProcessorState" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="_currentState" />.</returns>
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
                    StreamEvent @event = default;
                    while (@event == default && !cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            _currentState = await Catchup(_currentState, cancellationToken).ConfigureAwait(false);
                            @event = await FetchEventToProcess(_currentState, cancellationToken).ConfigureAwait(false);
                            if (@event == default) await Task.Delay(250).ConfigureAwait(false);
                        }
                        catch (EventStoreUnavailable)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                    }

                    if (cancellationToken.IsCancellationRequested) break;
                    _currentState = await ProcessEvent(@event, _currentState, cancellationToken).ConfigureAwait(false);
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    Logger.Warning(ex, "{streamProcessorId} for tenant {tenantId} failed", Identifier, _tenantId);
                }
            }
        }
    }
}
