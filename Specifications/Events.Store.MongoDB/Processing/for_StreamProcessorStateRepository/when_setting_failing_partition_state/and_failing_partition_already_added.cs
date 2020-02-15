// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_StreamProcessorStateRepository.when_setting_failing_partition_state
{
    public class and_failing_partition_already_added : given.all_dependencies
    {
        static StreamProcessorStateRepository repository;
        static Runtime.Events.Processing.PartitionId partition;
        static Runtime.Events.Processing.StreamProcessorId stream_processor_id;
        static Runtime.Events.Processing.StreamProcessorState initial_state;
        static Runtime.Events.Processing.FailingPartitionState new_failing_partition_state;
        static Runtime.Events.Processing.StreamProcessorState result;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            initial_state = Runtime.Events.Processing.StreamProcessorState.New;
            partition = Guid.NewGuid();
            new_failing_partition_state = new Runtime.Events.Processing.FailingPartitionState { Position = 1U, RetryTime = DateTimeOffset.UtcNow, Reason = "some new reason" };
            stream_processor_id = new Runtime.Events.Processing.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
            repository.GetOrAddNew(stream_processor_id).GetAwaiter().GetResult();
            repository.AddFailingPartition(stream_processor_id, partition, 0U, DateTimeOffset.UtcNow, "some old reason").GetAwaiter().GetResult();
        };

        Because of = () => result = repository.SetFailingPartitionState(stream_processor_id, partition, new_failing_partition_state).GetAwaiter().GetResult();

        It should_have_the_same_position = () => result.Position.ShouldEqual(initial_state.Position);
        It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
        It should_have_one_failing_partition_with_partition = () => result.FailingPartitions.TryGetValue(partition, out var state).ShouldBeTrue();
        It should_have_one_failing_partition_with_correct_position = () => result.FailingPartitions[partition].Position.ShouldEqual(new_failing_partition_state.Position);
        It should_have_one_failing_partition_with_retry_time = () => result.FailingPartitions[partition].RetryTime.ShouldEqual(new_failing_partition_state.RetryTime);
        It should_have_one_failing_partition_with_reason = () => result.FailingPartitions[partition].Reason.ShouldEqual(new_failing_partition_state.Reason);
    }
}