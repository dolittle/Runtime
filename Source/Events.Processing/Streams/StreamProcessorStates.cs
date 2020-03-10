// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStates" />.
    /// </summary>
    public class StreamProcessorStates : IStreamProcessorStates
    {
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStates"/> class.
        /// </summary>
        /// <param name="failingPartitions">The <see cref="IFailingPartitions" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessorStates(
            IFailingPartitions failingPartitions,
            IStreamProcessorStateRepository streamProcessorStates,
            ILogger logger)
        {
            FailingPartitions = failingPartitions;
            _streamProcessorStates = streamProcessorStates;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IFailingPartitions FailingPartitions { get; }

        /// <inheritdoc/>.
        public Task<StreamProcessorState> GetStoredStateFor(StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            _logger.Debug($"Getting stored stream processor state for Stream Processor '{streamProcessorId}'");
            return _streamProcessorStates.GetOrAddNew(streamProcessorId, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<StreamProcessorState> ProcessEventAndChangeStateFor(StreamProcessorId streamProcessorId, IEventProcessor eventProcessor, StreamEvent streamEvent, StreamProcessorState currentState, CancellationToken cancellationToken = default)
        {
            if (currentState.FailingPartitions.Keys.Contains(streamEvent.Partition))
            {
                return await IncrementPosition(streamProcessorId, currentState.Position, cancellationToken).ConfigureAwait(false);
            }

            _logger.Debug($"Processing event '{streamEvent.Event.Type.Id}' in partition '{streamEvent.Partition}' at position '{currentState.Position}' for Stream Processor '{streamProcessorId}'");

            var processingResult = await eventProcessor.Process(streamEvent.Event, streamEvent.Partition, cancellationToken).ConfigureAwait(false);
            if (processingResult is IRetryProcessingResult retryProcessingResult)
            {
                await FailingPartitions.AddFailingPartitionFor(
                    streamProcessorId,
                    streamEvent.Partition,
                    currentState.Position,
                    DateTimeOffset.UtcNow.AddMilliseconds(retryProcessingResult.RetryTimeout),
                    retryProcessingResult.FailureReason,
                    cancellationToken).ConfigureAwait(false);
            }
            else if (!processingResult.Succeeded)
            {
                await FailingPartitions.AddFailingPartitionFor(
                    streamProcessorId,
                    streamEvent.Partition,
                    currentState.Position,
                    DateTimeOffset.MaxValue,
                    processingResult.FailureReason,
                    cancellationToken).ConfigureAwait(false);
            }

            return await IncrementPosition(streamProcessorId, currentState.Position, cancellationToken).ConfigureAwait(false);
        }

        Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorId, StreamPosition position, CancellationToken cancellationToken)
        {
            _logger.Debug($"Stream Processor '{streamProcessorId}' is incrementing its position from '{position}' to '{position.Increment()}'");
            return _streamProcessorStates.IncrementPosition(streamProcessorId, cancellationToken);
        }
    }
}