// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions;

public class that_should_never_be_retried : given.all_dependencies
{
    static StreamProcessorState result;

    private Establish context = () =>
    {
        stream_processor_state = stream_processor_state.WithFailingPartition(first_failing_partition_id, new FailingPartitionState(
            first_failing_partition_state.Position,
            DateTimeOffset.MaxValue,
            first_failing_partition_state.Reason,
            first_failing_partition_state.ProcessingAttempts,
            DateTimeOffset.UtcNow));

        stream_processor_state = stream_processor_state.WithFailingPartition(second_failing_partition_id, new FailingPartitionState(
            second_failing_partition_state.Position,
            DateTimeOffset.MaxValue,
            second_failing_partition_state.Reason,
            second_failing_partition_state.ProcessingAttempts,
            DateTimeOffset.UtcNow));
        stream_processor_state_repository.Persist(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
    };

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult() as StreamProcessorState;

    It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
    It should_have_two_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(2);
    It should_have_failing_partition_with_first_failing_partition_id = () => result.FailingPartitions.ContainsKey(first_failing_partition_id).ShouldBeTrue();
    It should_have_failing_partition_with_second_failing_partition_id = () => result.FailingPartitions.ContainsKey(second_failing_partition_id).ShouldBeTrue();
    It should_not_change_first_failing_partition_position = () => failing_partition(first_failing_partition_id).Position.ShouldEqual(first_initial_failing_partition_position);
    It should_not_change_second_failing_partition_position = () => failing_partition(second_failing_partition_id).Position.ShouldEqual(second_initial_failing_partition_position);
    It should_not_change_first_failing_partition_reason = () => failing_partition(first_failing_partition_id).Reason.ShouldEqual(first_initial_failing_partition_reason);
    It should_not_change_second_failing_partition_reason = () => failing_partition(second_failing_partition_id).Reason.ShouldEqual(second_initial_failing_partition_reason);
    It should_not_change_first_failing_partition_retry_time = () => failing_partition(first_failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
    It should_not_change_second_failing_partition_retry_time = () => failing_partition(second_failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);

    It should_not_process_any_events = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    It should_not_retry_processing_process_any_events = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    static FailingPartitionState failing_partition(PartitionId partition_id) => result.FailingPartitions[partition_id];
}