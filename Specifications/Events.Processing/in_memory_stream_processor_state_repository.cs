// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    public class in_memory_stream_processor_state_repository : IStreamProcessorStateRepository
    {
        readonly IDictionary<StreamProcessorId, StreamProcessorState> states = new NullFreeDictionary<StreamProcessorId, StreamProcessorState>();

        public void Dispose()
        {
        }

        public Task<StreamProcessorState> GetOrAddNew(StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default)
        {
            states.TryGetValue(streamProcessorId, out var state);
            if (state == default)
            {
                state = StreamProcessorState.New;
                states.Add(streamProcessorId, state);
            }

            return Task.FromResult(state);
        }

        public Task<StreamProcessorState> SetNextEventToProcessPosition(StreamProcessorId streamProcessorId, StreamPosition position, CancellationToken cancellationToken = default)
        {
            var newState = states[streamProcessorId];
            newState.Position = position;
            states[streamProcessorId] = newState;

            return Task.FromResult(newState);
        }

        public Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition position, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken = default)
        {
            var newState = states[streamProcessorId];
            newState.FailingPartitions.Add(partitionId, new FailingPartitionState { Position = position, RetryTime = retryTime, Reason = reason, ProcessingAttempts = 1 });
            states[streamProcessorId] = newState;
            return Task.FromResult(newState);
        }

        public Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            var newState = states[streamProcessorId];
            newState.FailingPartitions.Remove(partitionId);
            states[streamProcessorId] = newState;
            return Task.FromResult(newState);
        }

        public Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState, CancellationToken cancellationToken = default)
        {
            var newState = states[streamProcessorId];
            newState.FailingPartitions[partitionId] = failingPartitionState;
            states[streamProcessorId] = newState;
            return Task.FromResult(newState);
        }
    }
}