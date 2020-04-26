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
    public abstract class StreamProcessor
    {
        Task _task;
        bool _stopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="Dolittle.Tenancy.TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The initial state of the <see cref="IStreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        protected StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            IStreamProcessorState initialState,
            IEventProcessor processor,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            Identifier = new StreamProcessorId(processor.Scope, processor.Identifier, sourceStreamId);
            CurrentState = initialState;
            TenantId = tenantId;
            Processor = processor;
            DetailedLogMessagePrefix = $"Stream Processor in Scope: {processor.Scope} on Stream: '{sourceStreamId}' with Processor: '{processor.Identifier}' with for Tenant: '{tenantId}'";
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="StreamProcessor"/>.
        /// </summary>
        public StreamProcessorId Identifier { get; }

        /// <summary>
        /// Gets or sets the current <see cref="IStreamProcessorState" />.
        /// </summary>
        /// <remarks>This <see cref="IStreamProcessorState" /> does not reflect the persisted state until the BeginProcessing.</remarks>
        protected IStreamProcessorState CurrentState { get; set; }

        /// <summary>
        /// Gets the <see cref="Dolittle.Tenancy.TenantId" /> that this <see cref="StreamProcessor" /> is processing for.
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
        /// Gets the <see cref="CancellationToken" />.
        /// </summary>
        protected CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets a value indicating whether the Stream Processor is stopping.
        /// </summary>
        /// <returns>True if stopping, false if not.</returns>
        protected bool IsStopping => _stopped || CancellationToken.IsCancellationRequested;

        /// <summary>
        /// Stops the stream processing.
        /// </summary>
        public void Stop()
        {
            _stopped = true;
        }

        /// <summary>
        /// Starts the stream processing.
        /// </summary>
        /// <returns>The stream processing task.</returns>
        public Task Start() => _task ?? (_task = BeginProcessing());

        /// <summary>
        /// Catchup on failing Events.
        /// </summary>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IStreamProcessorState" />.</returns>
        protected abstract Task<IStreamProcessorState> Catchup();

        /// <summary>
        /// Fetches the Event that is should be processed next.
        /// </summary>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamEvent" />.</returns>
        protected abstract Task<StreamEvent> FetchEventToProcess();

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
        /// <returns>A <see cref="Task"/> that, when returned, returns the new <see cref="CurrentState" />.</returns>
        protected virtual async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event)
        {
            var processingResult = await Processor.Process(@event.Event, @event.Partition, CancellationToken).ConfigureAwait(false);
            return await HandleProcessingResult(processingResult, @event).ConfigureAwait(false);
        }

        Task BeginProcessing()
        {
            return _task ?? Task.Run(
                async () =>
                {
                    try
                    {
                        do
                        {
                            StreamEvent @event = default;
                            while (@event == default && !IsStopping)
                            {
                                try
                                {
                                    CurrentState = await Catchup().ConfigureAwait(false);
                                    @event = await FetchEventToProcess().ConfigureAwait(false);
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

                            if (IsStopping) break;
                            CurrentState = await ProcessEvent(@event).ConfigureAwait(false);
                        }
                        while (!IsStopping);
                    }
                    catch (Exception ex)
                    {
                        if (!IsStopping)
                        {
                            Logger.Warning(ex, "{logPrefix} failed", DetailedLogMessagePrefix);
                        }
                    }
                });
        }

        Task<IStreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent)
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
    }
}