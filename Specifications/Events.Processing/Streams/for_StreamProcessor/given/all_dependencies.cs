// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.given
{
    public class all_dependencies
    {
        protected static TenantId tenant_id;
        protected static StreamId source_stream_id;
        protected static IStreamProcessorStateRepository stream_processor_state_repository;
        protected static Mock<IFetchEventsFromStreams> next_event_fetcher;
        protected static IStreamProcessorStates stream_processor_states;

        Establish context = () =>
        {
            var in_memory_stream_processor_state_repository = new in_memory_stream_processor_state_repository();

            tenant_id = Guid.NewGuid();
            source_stream_id = Guid.NewGuid();
            stream_processor_state_repository = in_memory_stream_processor_state_repository;
            next_event_fetcher = new Mock<IFetchEventsFromStreams>();
            stream_processor_states = new StreamProcessorStates(new FailingPartitions(stream_processor_state_repository, next_event_fetcher.Object, Mock.Of<ILogger>()), stream_processor_state_repository, Mock.Of<ILogger>());
        };
    }
}