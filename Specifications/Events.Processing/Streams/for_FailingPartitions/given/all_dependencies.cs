// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.given
{
    public class all_dependencies
    {
        protected static Mock<IStreamProcessorStateRepository> stream_processor_state_repository;
        protected static Mock<IFetchEventsFromStreams> events_fetcher;

        Establish context = () =>
        {
            stream_processor_state_repository = new Mock<IStreamProcessorStateRepository>();
            events_fetcher = new Mock<IFetchEventsFromStreams>();
        };
    }
}