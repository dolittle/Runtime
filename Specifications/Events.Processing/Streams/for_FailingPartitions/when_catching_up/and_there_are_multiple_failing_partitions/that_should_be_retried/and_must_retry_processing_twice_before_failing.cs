// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions.that_should_be_retried
{
    public class and_must_retry_processing_twice_before_failing : given.all_dependencies
    {
        const string new_failing_reason = "new failing reason";
        static Dictionary<PartitionId, int> num_processed;
        static StreamProcessorState result;

        Establish context = () =>
        {
            num_processed = new Dictionary<PartitionId, int>()
            {
                { first_failing_partition_id, 0 },
                { second_failing_partition_id, 0 }
            };
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<CommittedEvent, PartitionId, CancellationToken>((@event, partition, _) =>
                    Task.FromResult(num_processed[partition]++ >= 2 ? new FailedProcessingResult(new_failing_reason) as IProcessingResult : new RetryProcessingResult(0, new_failing_reason) as IProcessingResult));
        };

        Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, event_processor.Object, stream_processor_state).GetAwaiter().GetResult();

        It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
        It should_have_two_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(2);
        It should_have_failing_partition_with_first_failing_partition_id = () => result.FailingPartitions.ContainsKey(first_failing_partition_id).ShouldBeTrue();
        It should_have_failing_partition_with_second_failing_partition_id = () => result.FailingPartitions.ContainsKey(second_failing_partition_id).ShouldBeTrue();
        It should_not_change_first_failing_partition_position = () => result.FailingPartitions[first_failing_partition_id].Position.ShouldEqual(first_initial_failing_partition_position);
        It should_change_second_failing_partition_position_two_first_unprocessed_event_in_partition = () => result.FailingPartitions[second_failing_partition_id].Position.ShouldEqual(second_initial_failing_partition_position.Increment());
        It should_have_new_first_failing_partition_reason = () => result.FailingPartitions[first_failing_partition_id].Reason.ShouldEqual(new_failing_reason);
        It should_have_new_second_failing_partition_reason = () => result.FailingPartitions[second_failing_partition_id].Reason.ShouldEqual(new_failing_reason);
        It should_not_change_first_failing_partition_retry_time = () => result.FailingPartitions[first_failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_not_change_second_failing_partition_retry_time = () => result.FailingPartitions[second_failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_have_first_failing_partition_process_first_event_three_times = () => event_processor.Verify(_ => _.Process(committed_events[0], first_failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
        It should_have_second_failing_partition_process_second_event_three_times = () => event_processor.Verify(_ => _.Process(committed_events[1], second_failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
    }
}