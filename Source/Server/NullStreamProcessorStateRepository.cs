// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        public Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition position, DateTimeOffset retryTime)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> GetOrAddNew(StreamProcessorId streamProcessorId)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorId)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId)
        {
            return Task.FromResult(StreamProcessorState.New);
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState)
        {
            return Task.FromResult(StreamProcessorState.New);
        }
    }
}