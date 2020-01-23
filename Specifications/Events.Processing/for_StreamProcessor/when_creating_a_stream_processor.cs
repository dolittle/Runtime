// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor
{
    public class when_creating_a_stream_processor : given.all_dependencies
    {
        static StreamProcessor stream_processor;

        Because of = () => stream_processor = new StreamProcessor(tenant_id, source_stream_id, Processing.given.an_event_processor_mock().Object, get_stream_processor_state_repository, get_next_event_fetcher, Moq.Mock.Of<ILogger>());

        It should = () => stream_processor.Key.ShouldEqual();
    }
}