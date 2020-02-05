// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor.when_stream_processor_is_processing.and_stream_has_only_one_partition
{
    public class and_event_processing_failed_at_first_event : given.all_dependencies
    {
        static readonly PartitionId partition_id = PartitionId.NotSet;
        static readonly CommittedEvent first_event = Processing.given.a_committed_event;
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();
        static readonly Moq.Mock<IEventProcessor> event_processor_mock = Processing.given.an_event_processor_mock(event_processor_id, (new FailedProcessingResult(), first_event));
        static StreamProcessor stream_processor;
        static Task task;

        Establish context = () =>
        {
            next_event_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), 0)).Returns(Task.FromResult(new CommittedEventWithPartition { Event = first_event, PartitionId = ));
            stream_processor = new StreamProcessor(source_stream_id, event_processor_mock.Object, stream_processor_state_repository, next_event_fetcher.Object, Moq.Mock.Of<ILogger>());
        };

        Because of = () => stream_processor.BeginProcessing().Wait();

        It should_process_one_events = () => event_processor_mock.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>()), Moq.Times.Once());
        It should_process_first_event = () => event_processor_mock.Verify(_ => _.Process(first_event), Moq.Times.Once());
        It should_have_current_position_equal_zero = () => stream_processor.CurrentState.Position.ShouldEqual(new StreamPosition(0));
        It should_have_current_state_equal_stopping = () => stream_processor.CurrentState.State.ShouldEqual(StreamProcessingState.Stopping);
    }
}