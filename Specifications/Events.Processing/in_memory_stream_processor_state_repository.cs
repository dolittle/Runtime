// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Processing
{
    public class in_memory_stream_processor_state_repository : IStreamProcessorStateRepository
    {
        readonly IDictionary<StreamProcessorKey, StreamProcessorState> states = new NullFreeDictionary<StreamProcessorKey, StreamProcessorState>();

        public void Dispose()
        {
        }

        public Task<StreamProcessorState> Get(StreamProcessorKey streamProcessorKey)
        {
            if (!states.ContainsKey(streamProcessorKey)) return Task.FromResult<StreamProcessorState>(null);
            return Task.FromResult(states[streamProcessorKey]);
        }

        public Task Set(StreamProcessorKey streamProcessorKey, StreamProcessingState streamProcessingState, StreamPosition streamPosition) => Set(streamProcessorKey, new StreamProcessorState(streamProcessingState, streamPosition));

        public Task Set(StreamProcessorKey streamProcessorKey, StreamProcessorState streamProcessorState)
        {
            if (states.ContainsKey(streamProcessorKey)) states.Remove(streamProcessorKey);
            states.Add(streamProcessorKey, streamProcessorState);
            return Task.CompletedTask;
        }
    }
}