// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_RemoteFilterValidator.given
{
    public class all_dependencies
    {
        protected static StreamId source_stream;
        protected static StreamId target_stream;
        protected static Mock<IFilterProcessor<FilterDefinition>> filter_processor;
        protected static Mock<IFetchEventsFromStreams> events_fetcher;
        protected static Mock<IFetchEventTypesFromStreams> event_types_fetcher;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_states;
        protected static RemoteFilterValidator validator;

        Establish context = () =>
        {
            source_stream = Guid.NewGuid();
            target_stream = Guid.NewGuid();
            filter_processor = new Mock<IFilterProcessor<FilterDefinition>>();
            filter_processor.SetupGet(_ => _.Definition).Returns(new FilterDefinition(source_stream, target_stream, true));
            stream_processor_states = new Mock<IStreamProcessorStateRepository>();
            events_fetcher = new Mock<IFetchEventsFromStreams>();
            event_types_fetcher = new Mock<IFetchEventTypesFromStreams>();
            validator = new RemoteFilterValidator(events_fetcher.Object, event_types_fetcher.Object, stream_processor_states.Object, Mock.Of<ILogger>());
        };
    }
}