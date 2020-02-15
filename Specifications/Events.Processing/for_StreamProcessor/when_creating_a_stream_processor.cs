// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor
{
    public class when_creating_a_stream_processor : given.all_dependencies
    {
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();
        static readonly Moq.Mock<IEventProcessor> event_processor_mock = Processing.given.an_event_processor_mock(event_processor_id, new SucceededProcessingResult());
        static StreamProcessor stream_processor;

        Because of = () => stream_processor = new StreamProcessor(tenant_id, source_stream_id, event_processor_mock.Object, stream_processor_state_repository, next_event_fetcher.Object, Moq.Mock.Of<ILogger>());

        It should_have_the_correct_event_processor_id = () => stream_processor.EventProcessorId.ShouldEqual(event_processor_mock.Object.Identifier);
        It should_have_the_correct_key = () => stream_processor.Identifier.ShouldEqual(new StreamProcessorId(event_processor_mock.Object.Identifier, source_stream_id));
        It should_have_the_correct_initial_position = () => stream_processor.CurrentState.Position.ShouldEqual(StreamPosition.Start);
        It should_not_have_any_failing_partitions = () => stream_processor.CurrentState.FailingPartitions.Count.ShouldEqual(0);
    }
}