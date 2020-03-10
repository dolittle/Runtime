// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions.that_should_be_retried.and_succeeds_in_processing
{
    public class but_fails_on_last_event : given.all_dependencies
    {
        static string new_failure_reason = "new reason";
        static StreamProcessorState result;

        Establish context = () =>
        {
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<CommittedEvent, PartitionId, CancellationToken>((@event, partition, _) =>
                    @event == committed_events[2] ? Task.FromResult<IProcessingResult>(new FailedProcessingResult(new_failure_reason)) : Task.FromResult<IProcessingResult>(new SucceededProcessingResult()));
        };

        Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, event_processor.Object, stream_processor_state).GetAwaiter().GetResult();

        It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
        It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
        It should_have_failing_partition_with_correct_id = () => result.FailingPartitions.ContainsKey(first_failing_partition_id).ShouldBeTrue();
        It should_have_the_correct_position_on_failing_partition = () => result.FailingPartitions[first_failing_partition_id].Position.ShouldEqual(new StreamPosition(first_initial_failing_partition_position + 2));
        It should_have_new_failing_partition_reason = () => result.FailingPartitions[first_failing_partition_id].Reason.ShouldEqual(new_failure_reason);
        It should_not_retry_failing_partition = () => result.FailingPartitions[first_failing_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_have_processed_three_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
        It should_have_first_failing_partition_process_first_event_once = () => event_processor.Verify(_ => _.Process(committed_events[0], first_failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_have_second_failing_partition_process_second_event_once = () => event_processor.Verify(_ => _.Process(committed_events[1], second_failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_have_first_failing_process_process_third_event_once = () => event_processor.Verify(_ => _.Process(committed_events[2], first_failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}