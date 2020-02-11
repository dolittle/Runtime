// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents a null implementation of <see cref="IStreamProcessorStateRepository"/>.
    /// </summary>
    public class NullStreamProcessorStateRepository : IStreamProcessorStateRepository
    {
        /// <inheritdoc/>
        public Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition position, DateTimeOffset retryTime, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> GetOrAddNew(StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StreamProcessorState.New);
        }
    }
}