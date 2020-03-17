// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.that_should_be_retried
{
    public class and_must_retry_processing_twice_before_failing : given.all_dependencies
    {
        const string new_failing_reason = "new failing reason";
        static int num_processed = 0;
        static StreamProcessorState result;

        Establish context = () =>
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<CommittedEvent, PartitionId, CancellationToken>((@event, partition, _) => Task.FromResult(num_processed++ >= 2 ? new FailedProcessingResult(new_failing_reason) as IProcessingResult : new RetryProcessingResult(0, new_failing_reason) as IProcessingResult));

        Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, event_processor.Object, stream_processor_state).GetAwaiter().GetResult();

        It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
        It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
        It should_have_failing_partition_with_correct_id = () => result.FailingPartitions.ContainsKey(failing_partition_id).ShouldBeTrue();
        It should_not_change_failing_partition_position = () => result.FailingPartitions[failing_partition_id].Position.ShouldEqual(initial_failing_partition_position);
        It should_have_new_failing_partition_reason = () => result.FailingPartitions[failing_partition_id].Reason.ShouldEqual(new_failing_reason);
        It should_not_retry_failing_partition = () => result.FailingPartitions[failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_have_processed_event_once = () => event_processor.Verify(_ => _.Process(committed_events[0], failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
    }
}