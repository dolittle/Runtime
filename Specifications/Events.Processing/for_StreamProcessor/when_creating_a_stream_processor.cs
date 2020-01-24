// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor
{
    public class when_creating_a_stream_processor : given.all_dependencies
    {
        static readonly IEventProcessor event_processor = Processing.given.an_event_processor(Guid.NewGuid(), new SucceededProcessingResult());
        static StreamProcessor stream_processor;

        Because of = () => stream_processor = new StreamProcessor(tenant_id, source_stream_id, event_processor, stream_processor_state_repository, next_event_fetcher, Moq.Mock.Of<ILogger>());

        It should = () => stream_processor.Key.ShouldEqual(new StreamProcessorKey(event_processor.Identifier, source_stream_id));
    }
}