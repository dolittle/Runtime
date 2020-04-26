// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
     /// <summary>
    /// Represents a system that can process a stream of events.
    /// </summary>
    public class StreamProcessor : Streams.StreamProcessor
    {
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IPartitionedStreamProcessorStates _streamProcessorStates;
        readonly IFailingPartitions _failingPartitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IPartitionedStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="failingPartitions">The <see cref="IFailingPartitions" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            StreamProcessorState initialState,
            IEventProcessor processor,
            IPartitionedStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            IFailingPartitions failingPartitions,
            ILogger<StreamProcessor> logger,
            CancellationToken cancellationToken)
            : base(tenantId, sourceStreamId, initialState, processor, logger, cancellationToken)
        {
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStates = streamProcessorStates;
            _failingPartitions = failingPartitions;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event)
        {
            var currentState = CurrentState as StreamProcessorState;
            if (currentState.FailingPartitions.Keys.Contains(@event.Partition))
            {
                var newState = new StreamProcessorState(@event.Position + 1, currentState.FailingPartitions);
                await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
                return newState;
            }

            return await base.ProcessEvent(@event).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> Catchup()
        {
            var newState = await _failingPartitions.CatchupFor(Identifier, Processor, CurrentState as StreamProcessorState, CancellationToken).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override Task<StreamEvent> FetchEventToProcess() => _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, CancellationToken);

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnFailedProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent)
        {
            var newState = await _failingPartitions.AddFailingPartitionFor(
                Identifier,
                processedEvent.Partition,
                processedEvent.Position,
                DateTimeOffset.MaxValue,
                failedProcessing.FailureReason,
                CancellationToken.None).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnRetryProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent)
        {
            var newState = await _failingPartitions.AddFailingPartitionFor(
                Identifier,
                processedEvent.Partition,
                processedEvent.Position,
                DateTimeOffset.UtcNow.Add(failedProcessing.RetryTimeout),
                failedProcessing.FailureReason,
                CancellationToken.None).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnSuccessfulProcessing(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent)
        {
            var currentState = CurrentState as StreamProcessorState;
            var newState = new StreamProcessorState(processedEvent.Position + 1, currentState.FailingPartitions);
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken).ConfigureAwait(false);
            return newState;
        }
    }
}