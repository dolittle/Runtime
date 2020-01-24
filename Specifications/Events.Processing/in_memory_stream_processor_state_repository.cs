// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Processing
{
    public class in_memory_stream_processor_state_repository : IStreamProcessorStateRepository
    {
        readonly IDictionary<StreamProcessorKey, StreamProcessorState> states = new NullFreeDictionary<StreamProcessorKey, StreamProcessorState>();

        public void Dispose()
        {
        }

        public StreamProcessorState Get(StreamProcessorKey streamProcessorKey)
        {
            return states[streamProcessorKey];
        }

        public void Set(StreamProcessorKey streamProcessorKey, StreamProcessingState streamProcessingState, StreamPosition streamPosition)
        {
            Set(streamProcessorKey, new StreamProcessorState(streamProcessingState, streamPosition));
        }

        public void Set(StreamProcessorKey streamProcessorKey, StreamProcessorState streamProcessorState)
        {
            states.Add(streamProcessorKey, streamProcessorState);
        }
    }
}