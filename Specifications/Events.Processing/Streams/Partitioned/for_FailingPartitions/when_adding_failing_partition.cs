// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions;

public class when_adding_failing_partition : given.an_instance_of_failing_partitions
{
    static StreamProcessorId stream_processor_id;
    static PartitionId partition;
    static StreamPosition stream_position;
    static DateTimeOffset retry_time;
    static StreamProcessorState old_state;
    static string reason;
    static IStreamProcessorState result;

    Establish context = () =>
    {
        stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        partition = "a partition";
        stream_position = 0;
        retry_time = DateTimeOffset.Now;
        old_state = new StreamProcessorState(stream_position, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.UtcNow);
        reason = "";
    };

    Because of = () => result = failing_partitions.AddFailingPartitionFor(stream_processor_id, old_state, stream_position, partition, retry_time, reason, CancellationToken.None).GetAwaiter().GetResult();
    It should_persist_a_new_state = () => stream_processor_state_repository.Verify(_ => _.Persist(stream_processor_id, Moq.It.IsAny<IStreamProcessorState>(), Moq.It.IsAny<CancellationToken>()));
    It should_return_a_new_state = () => result.Should().NotBeNull();
    It should_return_a_state_of_the_expected_type = () => result.Should().BeOfType<StreamProcessorState>();
    It should_return_a_state_with_the_position_incremented_by_one = () => result.Position.Value.Should().Be(stream_position + 1);
    It should_return_a_state_with_that_is_partitioned = () => result.Partitioned.Should().BeTrue();
    It should_return_state_with_the_failing_partition = () => (result as StreamProcessorState).FailingPartitions.ContainsKey(partition).Should().BeTrue();
    It should_return_state_with_failing_partition_with_the_correct_position = () => (result as StreamProcessorState).FailingPartitions[partition].Position.Should().Be(stream_position);
    It should_return_state_with_failing_partition_with_the_correct_reason = () => (result as StreamProcessorState).FailingPartitions[partition].Reason.Should().Be(reason);
    It should_return_state_with_failing_partition_with_the_correct_number_of_processing_attempts = () => (result as StreamProcessorState).FailingPartitions[partition].ProcessingAttempts.Should().Be(1u);
    It should_return_state_with_failing_partition_with_the_correct_retry_time = () => (result as StreamProcessorState).FailingPartitions[partition].RetryTime.Should().Be(retry_time);
}