// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams.Unpartitioned
{
    /// <summary>
    /// Represents a system that can process a stream of events.
    /// </summary>
    public class StreamProcessor : Streams.StreamProcessor
    {
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IUnpartitionedStreamProcessorStates _streamProcessorStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IUnpartitionedStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            StreamProcessorState initialState,
            IEventProcessor processor,
            IUnpartitionedStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            ILogger<StreamProcessor> logger,
            CancellationToken cancellationToken)
            : base(tenantId, sourceStreamId, initialState, processor, logger, cancellationToken)
        {
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStates = streamProcessorStates;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> Catchup()
        {
            var currentState = CurrentState as StreamProcessorState;
            if (!currentState.IsFailing) return currentState;
            while (true)
            {
                if (!CanRetryProcessing(currentState.RetryTime))
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    CurrentState = await _streamProcessorStates.GetFor(Identifier, CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var @event = await _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, CancellationToken).ConfigureAwait(false);
                    return await RetryProcessingEvent(@event, currentState.FailureReason, currentState.ProcessingAttempts).ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc/>
        protected override Task<StreamEvent> FetchEventToProcess() => _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, CancellationToken);

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnFailedProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent)
        {
            var oldState = CurrentState as StreamProcessorState;
            var newState = new StreamProcessorState(oldState.Position, failedProcessing.FailureReason, DateTimeOffset.MaxValue, oldState.ProcessingAttempts + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnRetryProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent)
        {
            var oldState = CurrentState as StreamProcessorState;
            var newState = new StreamProcessorState(oldState.Position, failedProcessing.FailureReason, DateTimeOffset.MaxValue, oldState.ProcessingAttempts + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnSuccessfulProcessing(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent)
        {
            var newState = new StreamProcessorState(processedEvent.Position + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken).ConfigureAwait(false);
            return newState;
        }

        bool CanRetryProcessing(DateTimeOffset retryTime) => DateTimeOffset.UtcNow.CompareTo(retryTime) >= 0;
    }
}