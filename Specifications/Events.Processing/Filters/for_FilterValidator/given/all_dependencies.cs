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
        protected static Mock<IEventFetchers> events_fetchers;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_states;
        protected static FilterDefinition filter_definition;
        protected static RemoteFilterValidator validator;

        Establish context = () =>
        {
            source_stream = Guid.NewGuid();
            target_stream = Guid.NewGuid();
            filter_processor = new Mock<IFilterProcessor<FilterDefinition>>();
            filter_definition = new FilterDefinition(source_stream, target_stream, true);
            filter_processor.SetupGet(_ => _.Definition).Returns(filter_definition);
            stream_processor_states = new Mock<IStreamProcessorStateRepository>();
            events_fetchers = new Mock<IEventFetchers>();
            validator = new RemoteFilterValidator(events_fetchers.Object, stream_processor_states.Object, Mock.Of<ILogger>());
        };
    }
}