// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system that can process a stream of events.
    /// </summary>
    public class ScopedStreamProcessor : AbstractStreamProcessor
    {
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IStreamProcessorStates _streamProcessorStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedStreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public ScopedStreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            StreamProcessorState initialState,
            IEventProcessor processor,
            IStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            Action unregister,
            ILogger<ScopedStreamProcessor> logger,
            CancellationToken cancellationToken)
            : base(tenantId, sourceStreamId, initialState, processor, unregister, logger, cancellationToken)
        {
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStates = streamProcessorStates;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> Catchup(IStreamProcessorState currentState, CancellationToken cancellationToken)
        {
            var streamProcessorState = currentState as StreamProcessorState;
            if (!streamProcessorState.IsFailing) return streamProcessorState;
            while (true)
            {
                if (!CanRetryProcessing(streamProcessorState.RetryTime))
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(Identifier, cancellationToken).ConfigureAwait(false);
                    if (tryGetStreamProcessorState.Success) streamProcessorState = tryGetStreamProcessorState.Result as StreamProcessorState;
                }
                else
                {
                    var @event = await _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, streamProcessorState.Position, cancellationToken).ConfigureAwait(false);
                    return await RetryProcessingEvent(@event, streamProcessorState.FailureReason, streamProcessorState.ProcessingAttempts, streamProcessorState, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc/>
        protected override Task<StreamEvent> FetchEventToProcess(IStreamProcessorState currentState, CancellationToken cancellationToken) =>
            _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, currentState.Position, cancellationToken);

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState)
        {
            var oldState = currentState as StreamProcessorState;
            var newState = new StreamProcessorState(oldState.Position, failedProcessing.FailureReason, DateTimeOffset.MaxValue, oldState.ProcessingAttempts + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState)
        {
            var oldState = currentState as StreamProcessorState;
            var newState = new StreamProcessorState(oldState.Position, failedProcessing.FailureReason, DateTimeOffset.MaxValue, oldState.ProcessingAttempts + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent, IStreamProcessorState currentState)
        {
            var newState = new StreamProcessorState(processedEvent.Position + 1);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
            return newState;
        }

        bool CanRetryProcessing(DateTimeOffset retryTime) => DateTimeOffset.UtcNow.CompareTo(retryTime) >= 0;
    }
}