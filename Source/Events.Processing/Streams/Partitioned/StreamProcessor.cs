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
    public class StreamProcessor : AbstractStreamProcessor
    {
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IStreamProcessorStates _streamProcessorStates;
        readonly IFailingPartitions _failingPartitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="failingPartitions">The <see cref="IFailingPartitions" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="StreamProcessor" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            StreamProcessorState initialState,
            IEventProcessor processor,
            IStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            IFailingPartitions failingPartitions,
            Action unregister,
            ILogger<StreamProcessor> logger,
            CancellationToken cancellationToken)
            : base(tenantId, sourceStreamId, initialState, processor, unregister, logger, cancellationToken)
        {
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStates = streamProcessorStates;
            _failingPartitions = failingPartitions;
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> ProcessEvent(StreamEvent @event, CancellationToken cancellationToken)
        {
            var currentState = CurrentState as StreamProcessorState;
            if (currentState.FailingPartitions.Keys.Contains(@event.Partition))
            {
                var newState = new StreamProcessorState(@event.Position + 1, currentState.FailingPartitions);
                await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
                return newState;
            }

            return await base.ProcessEvent(@event, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> Catchup(CancellationToken cancellationToken)
        {
            var newState = await _failingPartitions.CatchupFor(Identifier, Processor, CurrentState as StreamProcessorState, cancellationToken).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        protected override Task<StreamEvent> FetchEventToProcess(CancellationToken cancellationToken) => _eventsFromStreamsFetcher.Fetch(Identifier.ScopeId, Identifier.SourceStreamId, CurrentState.Position, cancellationToken);

        /// <inheritdoc/>
        protected override async Task<IStreamProcessorState> OnFailedProcessing(FailedProcessing failedProcessing, StreamEvent processedEvent)
        {
            var newState = await _failingPartitions.AddFailingPartitionFor(
                Identifier,
                CurrentState as StreamProcessorState,
                processedEvent.Position,
                processedEvent.Partition,
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
                CurrentState as StreamProcessorState,
                processedEvent.Position,
                processedEvent.Partition,
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
            await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
            return newState;
        }
    }
}