// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions
{
    public class that_should_never_be_retried : given.all_dependencies
    {
        static StreamProcessorState result;

        Establish context = () =>
        {
            first_failing_partition_state.RetryTime = second_failing_partition_state.RetryTime = DateTimeOffset.MaxValue;
            stream_processor_state.FailingPartitions[first_failing_partition_id] = first_failing_partition_state;
            stream_processor_state.FailingPartitions[second_failing_partition_id] = second_failing_partition_state;
        };

        Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, event_processor.Object, stream_processor_state).GetAwaiter().GetResult();

        It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
        It should_have_two_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(2);
        It should_have_failing_partition_with_first_failing_partition_id = () => result.FailingPartitions.ContainsKey(first_failing_partition_id).ShouldBeTrue();
        It should_have_failing_partition_with_second_failing_partition_id = () => result.FailingPartitions.ContainsKey(second_failing_partition_id).ShouldBeTrue();
        It should_not_change_first_failing_partition_position = () => result.FailingPartitions[first_failing_partition_id].Position.ShouldEqual(first_initial_failing_partition_position);
        It should_not_change_second_failing_partition_position = () => result.FailingPartitions[second_failing_partition_id].Position.ShouldEqual(second_initial_failing_partition_position);
        It should_not_change_first_failing_partition_reason = () => result.FailingPartitions[first_failing_partition_id].Reason.ShouldEqual(first_initial_failing_partition_reason);
        It should_not_change_second_failing_partition_reason = () => result.FailingPartitions[second_failing_partition_id].Reason.ShouldEqual(second_initial_failing_partition_reason);
        It should_not_change_first_failing_partition_retry_time = () => result.FailingPartitions[first_failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_not_change_second_failing_partition_retry_time = () => result.FailingPartitions[second_failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_not_process_any_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
    }
}