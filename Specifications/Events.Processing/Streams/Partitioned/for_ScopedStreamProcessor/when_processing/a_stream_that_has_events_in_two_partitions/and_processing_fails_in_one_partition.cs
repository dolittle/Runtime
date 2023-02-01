// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_processing.a_stream_that_has_events_in_two_partitions;

public class and_processing_fails_in_one_partition : given.all_dependencies
{
    const string reason = "some reason";
    static PartitionId first_partition_id;
    static PartitionId second_partition_id;
    static StreamEvent first_event;
    static StreamEvent second_event;
    static StreamEvent third_event;
    static StreamEvent fourth_event;

    Establish context = () =>
    {
        first_partition_id = "first partition";
        second_partition_id = "second partition";
        first_event = new StreamEvent(committed_events.single(), StreamPosition.Start, Guid.NewGuid(), first_partition_id, true);
        second_event = new StreamEvent(committed_events.single(), 1u, Guid.NewGuid(), second_partition_id, true);
        third_event = new StreamEvent(committed_events.single(), 2u, Guid.NewGuid(), second_partition_id, true);
        fourth_event = new StreamEvent(committed_events.single(), 3u, Guid.NewGuid(), second_partition_id, true);
        event_processor
            .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), second_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        event_processor
            .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), first_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason)));
        setup_event_stream(first_event, second_event, third_event, fourth_event);
    };

    Because of = () => start_stream_processor_and_cancel_after(TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();

    It should_process_event_in_first_partition_once = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), first_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_process_three_events_in_second_partition = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), second_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
    It should_process_first_event = () => event_processor.Verify(_ => _.Process(first_event.Event, first_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_process_second_event = () => event_processor.Verify(_ => _.Process(second_event.Event, second_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_process_third_event = () => event_processor.Verify(_ => _.Process(third_event.Event, second_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_process_fourth_event = () => event_processor.Verify(_ => _.Process(fourth_event.Event, second_partition_id, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_not_retry_processing = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    It should_have_persisted_the_correct_position = () => current_stream_processor_state.Position.ShouldEqual(new StreamPosition(4));
    It should_have_persisted_state_with_one_failing_partition = () => current_stream_processor_state.FailingPartitions.Count.ShouldEqual(1);
    It should_have_persisted_state_with_correct_first_failing_partition = () => current_stream_processor_state.FailingPartitions.ContainsKey(first_partition_id).ShouldBeTrue();
    It should_have_persisted_correct_position_on_the_first_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].Position.ShouldEqual(new StreamPosition(0));
    It should_have_persisted_correct_retry_time_on_the_first_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
    It should_have_persisted_correct_reason_on_the_first_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].Reason.ShouldEqual(reason);
}