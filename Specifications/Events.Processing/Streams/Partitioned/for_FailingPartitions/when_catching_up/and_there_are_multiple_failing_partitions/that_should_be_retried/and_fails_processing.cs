// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions.that_should_be_retried;

public class and_fails_processing : given.all_dependencies
{
    const string new_failing_reason = "new failing reason";
    static StreamProcessorState result;

    Establish context = () =>
    {
        stream_processor_state_repository.Persist(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
        
        event_processor
            .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(),
                Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(new_failing_reason)));
    };

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult() as StreamProcessorState;

    It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
    It should_have_two_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(2);
    It should_have_failing_partition_with_first_failing_partition_id = () => result.FailingPartitions.ContainsKey(first_failing_partition_id).ShouldBeTrue();
    It should_have_failing_partition_with_second_failing_partition_id = () => result.FailingPartitions.ContainsKey(second_failing_partition_id).ShouldBeTrue();
    It should_not_change_first_failing_partition_position = () => failing_partition(first_failing_partition_id).Position.ShouldEqual(first_initial_failing_partition_position);
    It should_change_second_failing_partition_position_two_first_unprocessed_event_in_partition = () => failing_partition(second_failing_partition_id).Position.ShouldEqual(new ProcessingPosition(1,0));
    It should_have_new_first_failing_partition_reason = () => failing_partition(first_failing_partition_id).Reason.ShouldEqual(new_failing_reason);
    It should_have_new_second_failing_partition_reason = () => failing_partition(second_failing_partition_id).Reason.ShouldEqual(new_failing_reason);
    It should_not_change_first_failing_partition_retry_time = () => failing_partition(first_failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
    It should_not_change_second_failing_partition_retry_time = () => failing_partition(second_failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);

    It should_have_first_failing_partition_process_first_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[0].Event,
            first_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    It should_have_second_failing_partition_process_second_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[1].Event,
            second_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    static FailingPartitionState failing_partition(PartitionId partition_id) => result.FailingPartitions[partition_id];
}