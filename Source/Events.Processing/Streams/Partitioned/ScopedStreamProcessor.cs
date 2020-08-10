// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractScopedStreamProcessor" /> that processes an partitioned stream of events.
    /// </summary>
    public class ScopedStreamProcessor : AbstractScopedStreamProcessor
    {
        readonly IResilientStreamProcessorStateRepository _streamProcessorStates;
        readonly IFailingPartitions _failingPartitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedStreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="failingPartitions">The <see cref="IFailingPartitions" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        public ScopedStreamProcessor(
            TenantId tenantId,
            IStreamProcessorId streamProcessorId,
            StreamProcessorState initialState,
            IEventProcessor processor,
            IResilientStreamProcessorStateRepository streamProcessorStates,
            ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
            IFailingPartitions failingPartitions,
            IAsyncPolicyFor<ICanFetchEventsFromStream> eventsFetcherPolicy,
            ILogger<ScopedStreamProcessor> logger)
            : base(tenantId, streamProcessorId, initialState, processor, eventsFromStreamsFetcher, eventsFetcherPolicy, logger)
        {
            _streamProcessorStates = streamProcessorStates;
            _failingPartitions = failingPartitions;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event, IStreamProcessorState currentState, CancellationToken cancellationToken)
        {
            var streamProcessorState = currentState as StreamProcessorState;
            if (streamProcessorState.FailingPartitions.Keys.Contains(@event.Partition))
            {
                var newState = new StreamProcessorState(@event.Position + 1, streamProcessorState.FailingPartitions, streamProcessorState.LastSuccessfullyProcessed);
                await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
                return newState;
            }

            return await base.ProcessEvent(@event, streamProcessorState, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Task<IStreamProcessorState> Catchup(IStreamProcessorState currentState, CancellationToken cancellationToken) =>
            _failingPartitions.CatchupFor(Identifier, currentState as StreamProcessorState, cancellationToken);

        /// <inheritdoc/>
        protected override Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState) =>
            _failingPartitions.AddFailingPartitionFor(
                Identifier,
                currentState as StreamProcessorState,
                processedEvent.Position,
                processedEvent.Partition,
                DateTimeOffset.MaxValue,
                failedProcessing.FailureReason,
                CancellationToken.None);

        /// <inheritdoc/>
        protected override Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent, IStreamProcessorState currentState) =>
            _failingPartitions.AddFailingPartitionFor(
                Identifier,
                currentState as StreamProcessorState,
                processedEvent.Position,
                processedEvent.Partition,
                DateTimeOffset.UtcNow.Add(failedProcessing.RetryTimeout),
                failedProcessing.FailureReason,
                CancellationToken.None);

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent, IStreamProcessorState currentState)
        {
            var oldState = currentState as StreamProcessorState;
            var newState = new StreamProcessorState(processedEvent.Position + 1, oldState.FailingPartitions, DateTimeOffset.UtcNow);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
            return newState;
        }
    }
}