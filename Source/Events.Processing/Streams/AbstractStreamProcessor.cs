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
    /// Represents a system that can process a stream of events.
    /// </summary>
    public abstract class AbstractStreamProcessor
    {
        readonly CancellationToken _externalCancellationToken;
        readonly Action _unregister;
        bool _started;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="Dolittle.Tenancy.TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The initial state of the <see cref="IStreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="AbstractStreamProcessor" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        protected AbstractStreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            IStreamProcessorState initialState,
            IEventProcessor processor,
            Action unregister,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            Identifier = new StreamProcessorId(processor.Scope, processor.Identifier, sourceStreamId);
            CurrentState = initialState;
            TenantId = tenantId;
            Processor = processor;
            DetailedLogMessagePrefix = $"Stream Processor in Scope: {processor.Scope} on Stream: '{sourceStreamId}' with Processor: '{processor.Identifier}' with for Tenant: '{tenantId}'";
            Logger = logger;

            _externalCancellationToken = cancellationToken;
            _unregister = unregister;
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="AbstractStreamProcessor"/>.
        /// </summary>
        public StreamProcessorId Identifier { get; }

        /// <summary>
        /// Gets or sets the current <see cref="IStreamProcessorState" />.
        /// </summary>
        /// <remarks>This <see cref="IStreamProcessorState" /> does not reflect the persisted state until the BeginProcessing.</remarks>
        protected IStreamProcessorState CurrentState { get; set; }

        /// <summary>
        /// Gets the <see cref="Dolittle.Tenancy.TenantId" /> that this <see cref="AbstractStreamProcessor" /> is processing for.
        /// </summary>
        protected TenantId TenantId { get; }

        /// <summary>
        /// Gets the <see cref="IEventProcessor" />.
        /// </summary>
        protected IEventProcessor Processor { get; }

        /// <summary>
        /// Gets the detailed log message prefix.
        /// </summary>
        protected string DetailedLogMessagePrefix { get; }

        /// <summary>
        /// Gets the <see cref="ILogger" />.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Stops the stream processing.
        /// </summary>
        public void Stop() => _internalCancellationTokenSource.Cancel();

        /// <summary>
        /// Starts the stream processing.
        /// </summary>
        public void Start()
        {
            if (_started) throw new StreamProcessorAlreadyProcessingStream(Identifier);
            _started = true;
            _ = BeginProcessing();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (!_internalCancellationTokenSource.IsCancellationRequested)
                    {
                        _internalCancellationTokenSource.Cancel();
                    }

                    _internalCancellationTokenSource.Dispose();
                    _internalCancellationTokenSource = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Catchup on failing Events.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IStreamProcessorState" />.</returns>
        protected abstract Task<IStreamProcessorState> Catchup(CancellationToken cancellationToken);

        /// <summary>
        /// Fetches the Event that is should be processed next.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamEvent" />.</returns>
        protected abstract Task<StreamEvent> FetchEventToProcess(CancellationToken cancellationToken);

        /// <summary>
        ///  Gets the new <see cref="CurrentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the processing of the <see cref="StreamEvent" /> succeeded.
        /// </summary>
        /// <param name="successfulProcessing">The <see cref="SuccessfulProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnSuccessfulProcessing(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent);

        /// <summary>
        ///  Gets the new <see cref="CurrentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnRetryProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent);

        /// <summary>
        ///  Gets the new <see cref="CurrentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should not be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<IStreamProcessorState> OnFailedProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent);

        /// <summary>
        /// Process the <see cref="StreamEvent" /> and get the new <see cref="CurrentState" />.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="CurrentState" />.</returns>
        protected virtual async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event, CancellationToken cancellationToken)
        {
            var processingResult = await Processor.Process(@event.Event, @event.Partition, cancellationToken).ConfigureAwait(false);
            return await HandleProcessingResult(processingResult, @event).ConfigureAwait(false);
        }

        /// <summary>
        /// Process the <see cref="StreamEvent" /> and get the new <see cref="CurrentState" />.
        /// </summary>
        /// <param name="event">The <see cref="StreamEvent" />.</param>
        /// <param name="failureReason">The reason for why processing failed the last time.</param>
        /// <param name="processingAttempts">The number of times that this event has been processed before.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="CurrentState" />.</returns>
        protected async Task<IStreamProcessorState> RetryProcessingEvent(StreamEvent @event, string failureReason, uint processingAttempts, CancellationToken cancellationToken)
        {
            var processingResult = await Processor.Process(@event.Event, @event.Partition, failureReason, processingAttempts - 1, cancellationToken).ConfigureAwait(false);
            return await HandleProcessingResult(processingResult, @event).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle the <see cref="IProcessingResult" /> from the procssing of a <see cref="StreamEvent" />..
        /// </summary>
        /// <param name="processingResult">The <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The processed <see cref="StreamEvent" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent)
        {
            if (processingResult.Retry)
            {
                return OnRetryProcessing(processingResult as FailedProcessing, processedEvent);
            }
            else if (!processingResult.Succeeded)
            {
                return OnFailedProcessing(processingResult as FailedProcessing, processedEvent);
            }

            return OnSuccessfulProcessing(processingResult as SuccessfulProcessing, processedEvent);
        }

        async Task BeginProcessing()
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationTokenSource.Token, _externalCancellationToken);
            var token = cts.Token;
            try
            {
                do
                {
                    StreamEvent @event = default;
                    while (@event == default && !token.IsCancellationRequested)
                    {
                        try
                        {
                            CurrentState = await Catchup(token).ConfigureAwait(false);
                            @event = await FetchEventToProcess(cts.Token).ConfigureAwait(false);
                        }
                        catch (NoEventInStreamAtPosition)
                        {
                            await Task.Delay(250).ConfigureAwait(false);
                        }
                        catch (EventStoreUnavailable)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                    }

                    if (token.IsCancellationRequested) break;
                    CurrentState = await ProcessEvent(@event, cts.Token).ConfigureAwait(false);
                }
                while (!token.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger.Warning(ex, "{logPrefix} failed", DetailedLogMessagePrefix);
                }
            }
            finally
            {
                _unregister();
            }
        }
    }
}
