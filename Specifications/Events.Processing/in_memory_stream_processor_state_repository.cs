// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Processing
{
    public class in_memory_stream_processor_state_repository : IStreamProcessorStateRepository
    {
        readonly IDictionary<StreamProcessorId, StreamProcessorState> states = new NullFreeDictionary<StreamProcessorId, StreamProcessorState>();

        public void Dispose()
        {
        }

        public Task<StreamProcessorState> GetOrAddNew(StreamProcessorId streamProcessorKey)
        {
            states.TryGetValue(streamProcessorKey, out var state);
            if (state == default) state = StreamProcessorState.New;
            return Task.FromResult(state);
        }

        public Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorKey)
        {
            throw new NotImplementedException();
        }

        public Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, StreamProcessorState currentState, PartitionId partitionId, DateTimeOffset retryTime)
        {
            throw new NotImplementedException();
        }

        public Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId)
        {
            throw new NotImplementedException();
        }

        public Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState)
        {
            throw new NotImplementedException();
        }
    }
}