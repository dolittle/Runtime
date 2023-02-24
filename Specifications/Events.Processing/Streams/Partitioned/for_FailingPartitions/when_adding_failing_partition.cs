// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions;

public class when_adding_failing_partition : given.an_instance_of_failing_partitions
{
    static StreamProcessorId stream_processor_id;
    static PartitionId partition;
    static ProcessingPosition processing_position;
    static DateTimeOffset retry_time;
    static StreamProcessorState old_state;
    static string reason;
    static IStreamProcessorState result;

    static StreamEvent stream_event;

    
    Establish context = () =>
    {

        stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        partition = "a partition";
        stream_event = new StreamEvent(committed_events.single(), 0, Guid.Empty, partition, false);
        processing_position = ProcessingPosition.Initial;
        retry_time = DateTimeOffset.Now;
        old_state = new StreamProcessorState(processing_position, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.UtcNow);
        reason = "";
    };

    private Because of = () => result = old_state.WithFailure(new FailedProcessing(reason, true, TimeSpan.Zero), stream_event, retry_time);
    It should_return_a_new_state = () => result.ShouldNotBeNull();
    It should_return_a_state_of_the_expected_type = () => result.ShouldBeOfExactType<StreamProcessorState>();
    It should_return_a_state_with_the_position_incremented_by_one = () => result.Position.StreamPosition.Value.ShouldEqual(processing_position.StreamPosition + 1);
    It should_return_a_state_with_that_is_partitioned = () => result.Partitioned.ShouldBeTrue();
    It should_return_state_with_the_failing_partition = () => (result as StreamProcessorState).FailingPartitions.ContainsKey(partition).ShouldBeTrue();
    It should_return_state_with_failing_partition_with_the_correct_position = () => (result as StreamProcessorState).FailingPartitions[partition].Position.ShouldEqual(processing_position);
    It should_return_state_with_failing_partition_with_the_correct_reason = () => (result as StreamProcessorState).FailingPartitions[partition].Reason.ShouldEqual(reason);
    It should_return_state_with_failing_partition_with_the_correct_number_of_processing_attempts = () => (result as StreamProcessorState).FailingPartitions[partition].ProcessingAttempts.ShouldEqual(1u);
    It should_return_state_with_failing_partition_with_the_correct_retry_time = () => (result as StreamProcessorState).FailingPartitions[partition].RetryTime.ShouldEqual(retry_time);
}