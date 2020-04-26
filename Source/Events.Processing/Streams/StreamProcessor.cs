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
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        protected StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            IEventProcessor processor,
            IStreamProcessorStateRepository streamProcessorStateRepository,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            StreamProcessorStateRepository = streamProcessorStateRepository;

            Identifier = new StreamProcessorId(processor.Scope, processor.Identifier, sourceStreamId);
            CurrentState = StreamProcessorState.New;
            Processor = processor;
            TenantId = tenantId;
            DetailedLogMessagePrefix = $"Stream Processor for Event Processor: '{processor.Identifier}' in Scope: {processor.Scope} with on Stream: '{sourceStreamId}' for Tenant: '{tenantId}'";
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="StreamProcessor"/>.
        /// </summary>
        public StreamProcessorId Identifier { get; }

        /// <summary>
        /// Gets or sets the current <see cref="StreamProcessorState" />.
        /// </summary>
        /// <remarks>This <see cref="StreamProcessorState" /> does not reflect the persisted state until the BeginProcessing.</remarks>
        protected StreamProcessorState CurrentState { get; set; }

        /// <summary>
        /// Gets the <see cref="Dolittle.Tenancy.TenantId" /> that this <see cref="StreamProcessor" /> is processing for.
        /// </summary>
        protected TenantId TenantId { get; }

        /// <summary>
        /// Gets the <see cref="IStreamProcessorStateRepository" />.
        /// </summary>
        protected IStreamProcessorStateRepository StreamProcessorStateRepository { get; }

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
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="StreamProcessorState" />.</returns>
        protected abstract Task<StreamProcessorState> Catchup();

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
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<StreamProcessorState> OnSuccessfulProcessing(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent, CancellationToken cancellationToken);

        /// <summary>
        ///  Gets the new <see cref="CurrentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<StreamProcessorState> OnRetryProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent, CancellationToken cancellationToken);

        /// <summary>
        ///  Gets the new <see cref="CurrentState" /> after hanling the event of a <see cref="IProcessingResult" /> that signifies that the <see cref="StreamEvent" /> should not be processed again.
        /// </summary>
        /// <param name="failedProcessing">The <see cref="FailedProcessing" /> <see cref="IProcessingResult" />.</param>
        /// <param name="processedEvent">The <see cref="StreamEvent" /> that was processed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="CurrentState" />.</returns>
        protected abstract Task<StreamProcessorState> OnFailedProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent, CancellationToken cancellationToken);

        Task BeginProcessing()
        {
            return _task ?? Task.Run(
                async () =>
                {
                    try
                    {
                        if (IsStopping) return;
                        CurrentState = await StreamProcessorStateRepository.GetOrAddNew(Identifier, CancellationToken).ConfigureAwait(false);
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
                            var processingResult = await ProcessEvent(@event).ConfigureAwait(false);
                            CurrentState = await HandleProcessingResult(processingResult, @event).ConfigureAwait(false);
                        }
                        while (!IsStopping);
                    }
                    catch (Exception ex)
                    {
                        if (!IsStopping)
                        {
                            Logger.Error($"{DetailedLogMessagePrefix} failed - {ex}");
                        }
                    }
                });
        }

        Task<StreamProcessorState> HandleProcessingResult(IProcessingResult processingResult, StreamEvent processedEvent)
        {
            if (processingResult.Retry)
            {
                return OnRetryProcessing(processingResult as FailedProcessing, processedEvent, CancellationToken.None);
            }
            else if (!processingResult.Succeeded)
            {
                return OnFailedProcessing(processingResult as FailedProcessing, processedEvent, CancellationToken.None);
            }

            return OnSuccessfulProcessing(processingResult as SuccessfulProcessing, processedEvent, CancellationToken.None);
        }

        Task<IProcessingResult> ProcessEvent(StreamEvent @event)
        {
            return Processor.Process(@event.Event, @event.Partition, CancellationToken);
        }
    }
}