// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStates" />.
    /// </summary>
    public class StreamProcessorStates : IStreamProcessorStates
    {
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IAsyncPolicyFor<StreamProcessorStates> _policy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStates"/> class.
        /// </summary>
        /// <param name="failingPartitions">The <see cref="IFailingPartitions" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}>" /> the <see cref="StreamProcessorStates" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessorStates(
            IFailingPartitions failingPartitions,
            IStreamProcessorStateRepository streamProcessorStates,
            IAsyncPolicyFor<StreamProcessorStates> policy,
            ILogger logger)
        {
            FailingPartitions = failingPartitions;
            _streamProcessorStates = streamProcessorStates;
            _policy = policy;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IFailingPartitions FailingPartitions { get; }

        /// <inheritdoc/>
        public async Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, PartitionId partition, StreamPosition position, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken = default)
        {
            await FailingPartitions.AddFailingPartitionFor(streamProcessorId, partition, position, retryTime, reason, cancellationToken).ConfigureAwait(false);
            return await _policy.ExecuteAsync(
                cancellationToken => IncrementPosition(streamProcessorId, position, cancellationToken),
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<StreamProcessorState> HandleProcessingResult(StreamProcessorId streamProcessorId, IProcessingResult processingResult, StreamPosition currentStreamPosition, CancellationToken cancellationToken = default)
        {
            if (processingResult.Succeeded)
            {
                CurrentState = await IncrementPosition().ConfigureAwait(false);
            }
            else if (processingResult is IRetryProcessingResult retryProcessingResult)
            {
                CurrentState = await AddFailingPartitionAndIncrementPosition(streamEvent.Partition, retryProcessingResult.RetryTimeout, retryProcessingResult.FailureReason).ConfigureAwait(false);
            }
            else
            {
                CurrentState = await AddFailingPartitionAndIncrementPosition(streamEvent.Partition, DateTimeOffset.MaxValue, processingResult.FailureReason).ConfigureAwait(false);
            }
        }

        Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorId, StreamPosition position, CancellationToken cancellationToken )
        {
            _logger.Debug($"Stream Processor '{streamProcessorId}' is incrementing its position from '{position}' to '{position.Increment()}'");
            return _streamProcessorStates.IncrementPosition(streamProcessorId, cancellationToken);
        }
    }
}