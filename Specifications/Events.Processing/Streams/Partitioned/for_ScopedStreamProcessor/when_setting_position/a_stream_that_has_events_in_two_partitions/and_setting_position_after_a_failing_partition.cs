// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_setting_position.a_stream_that_has_events_in_two_partitions;

public class and_processing_fails_at_second_event_in_both_partitions : all_dependencies
{
    const string reason = "some reason";
    static PartitionId first_partition_id;
    static PartitionId second_partition_id;
    static StreamEvent first_event;
    static StreamEvent second_event;

    Establish context = () =>
    {
        first_partition_id = "first-partition-id";
        second_partition_id = "SECOND PARTITION ID";
        first_event = new StreamEvent(committed_events.single(), StreamPosition.Start, Guid.NewGuid(), first_partition_id, true);
        second_event = new StreamEvent(committed_events.single(), 1u, Guid.NewGuid(), second_partition_id, true);
        event_processor
            .Setup(_ => _.Process(first_event.Event, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason)));
        event_processor
            .Setup(_ => _.Process(second_event.Event, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        setup_event_stream(first_event, second_event);
    };

    Because of = () => start_stream_processor_set_position_after_and_cancel_after(TimeSpan.FromMilliseconds(100), new ProcessingPosition(1,1), action_to_perform_before_reprocessing.Object, TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();
        
    It should_process_first_event_once = () => event_processor.Verify(_ => _.Process(first_event.Event, first_partition_id, first_event.Position, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_process_second_event_twice = () => event_processor.Verify(_ => _.Process(second_event.Event, second_partition_id, second_event.Position, Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
    It should_not_retry_processing = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
    It should_have_persisted_the_correct_position = () => current_stream_processor_state.Position.StreamPosition.ShouldEqual(new StreamPosition(2));
    It should_have_persisted_state_with_one_failing_partitions = () => current_stream_processor_state.FailingPartitions.Count.ShouldEqual(1);
    It should_have_persisted_state_with_correct_failing_partition = () => current_stream_processor_state.FailingPartitions.ContainsKey(first_partition_id).ShouldBeTrue();
    It should_have_persisted_correct_position_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].Position.ShouldEqual(ProcessingPosition.Initial);
    It should_have_persisted_correct_retry_time_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
    It should_have_persisted_correct_reason_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[first_partition_id].Reason.ShouldEqual(reason);
    It should_perform_the_action = () => action_to_perform_before_reprocessing.Verify(_ => _(tenant_id, Moq.It.IsAny<CancellationToken>()), Times.Once);
}