// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.that_should_be_retried;

public class and_must_retry_processing_twice_before_failing : given.all_dependencies
{
    const string new_failing_reason = "new failing reason";
    static int num_processed = 0;
    static StreamProcessorState result;

    Establish context = () =>
        event_processor
            .Setup(_ => _.Process(
                Moq.It.IsAny<CommittedEvent>(),
                Moq.It.IsAny<PartitionId>(),
                Moq.It.IsAny<StreamPosition>(),
                Moq.It.IsAny<string>(),
                Moq.It.IsAny<uint>(),
                Moq.It.IsAny<ExecutionContext>(),
                Moq.It.IsAny<CancellationToken>()))
            .Returns<CommittedEvent, PartitionId, StreamPosition, string, uint, ExecutionContext, CancellationToken>((@event, partition, position, reason, _, retryCount, _) => Task.FromResult<IProcessingResult>(num_processed++ >= 2 ? new FailedProcessing(new_failing_reason) : new FailedProcessing(new_failing_reason, true, TimeSpan.Zero)));

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult() as StreamProcessorState;

    It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
    It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
    It should_have_failing_partition_with_correct_id = () => result.FailingPartitions.ContainsKey(failing_partition_id).ShouldBeTrue();
    It should_not_change_failing_partition_position = () => failing_partition(failing_partition_id).Position.ShouldEqual(initial_failing_partition_position);
    It should_have_new_failing_partition_reason = () => failing_partition(failing_partition_id).Reason.ShouldEqual(new_failing_reason);
    It should_not_retry_failing_partition = () => failing_partition(failing_partition_id).RetryTime.ShouldEqual(DateTimeOffset.MaxValue);

    It should_have_retried_processing_three_times = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
    
    It should_have_retried_processing_first_event_three_times = () => event_processor.Verify(
        _ => _.Process(
            eventStream[(int)failing_partition(failing_partition_id).Position.StreamPosition.Value].Event,
            failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
   

    static FailingPartitionState failing_partition(PartitionId partition_id) => result.FailingPartitions[partition_id];
}