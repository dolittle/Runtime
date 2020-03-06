// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.given
{
    public class all_dependencies
    {
        protected static Mock<IFailingPartitions> failing_partitions;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_state_repository;
        protected static StreamProcessorStates stream_processor_states;

        Establish context = () =>
        {
            failing_partitions = new Mock<IFailingPartitions>();
            stream_processor_state_repository = new Mock<IStreamProcessorStateRepository>();
            stream_processor_states = new StreamProcessorStates(failing_partitions.Object, stream_processor_state_repository.Object, Mock.Of<ILogger>());
        };
    }
}