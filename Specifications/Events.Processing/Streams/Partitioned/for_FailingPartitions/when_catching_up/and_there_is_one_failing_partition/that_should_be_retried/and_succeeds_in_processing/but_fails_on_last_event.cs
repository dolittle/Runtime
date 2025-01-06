// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.that_should_be_retried.and_succeeds_in_processing;

public class but_fails_on_last_event : given.all_dependencies
{
    static string new_failure_reason = "new reason";
    static StreamProcessorState result;
    

    Establish context = () =>
    {
        event_processor
            .Setup(_ => _.Process(eventStream[0].Event, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        event_processor
            .Setup(_ => _.Process(eventStream[1].Event, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        event_processor
            .Setup(_ => _.Process(eventStream[2].Event, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(new_failure_reason)));
    };

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult() as StreamProcessorState;

    It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
    It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
    It should_have_failing_partition_with_correct_id = () => result.FailingPartitions.ContainsKey(failing_partition_id).ShouldBeTrue();
    It should_have_the_correct_position_on_failing_partition = () => failing_partition(failing_partition_id).Position.StreamPosition.ShouldEqual(new StreamPosition(initial_failing_partition_position.StreamPosition + 2));
    It should_have_new_failing_partition_reason = () => failing_partition(failing_partition_id).Reason.ShouldEqual(new_failure_reason);
    It should_not_retry_failing_partition = () => failing_partition(failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);

    It should_not_process_any_events = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    It should_have_retried_processing_three_events = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));

    It should_have_processed_first_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[0].Event,
            failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    It should_have_processed_second_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[1].Event,
            failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    It should_have_processed_third_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[2].Event,
            failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    static FailingPartitionState failing_partition(PartitionId partition_id) => result.FailingPartitions[partition_id];
}