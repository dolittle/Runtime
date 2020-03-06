// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.given
{
    public class all_dependencies
    {
        protected static Mock<IStreamProcessorStateRepository> stream_processor_state_repository;
        protected static Mock<IFetchEventsFromStreams> events_fetcher;
        protected static FailingPartitions failing_partitions;

        Establish context = () =>
        {
            stream_processor_state_repository = new Mock<IStreamProcessorStateRepository>();
            events_fetcher = new Mock<IFetchEventsFromStreams>();
            failing_partitions = new FailingPartitions(stream_processor_state_repository.Object, events_fetcher.Object, Mock.Of<ILogger>());
        };
    }
}