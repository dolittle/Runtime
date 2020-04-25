// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.for_StreamProcessorStateRepository.when_adding_failing_partition
{
    public class and_failing_partition_is_not_already_added : given.all_dependencies
    {
        const string reason = "some reason";
        static StreamProcessorStateRepository repository;
        static PartitionId partition;
        static Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id;
        static Runtime.Events.Processing.Streams.StreamProcessorState initial_state;
        static Runtime.Events.Processing.Streams.StreamProcessorState result;
        static StreamPosition failing_partition_position;
        static DateTimeOffset failing_partition_retry_time;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            partition = Guid.NewGuid();
            initial_state = Runtime.Events.Processing.Streams.StreamProcessorState.New;
            failing_partition_position = 0U;
            failing_partition_retry_time = DateTimeOffset.UtcNow;
            stream_processor_id = new Runtime.Events.Processing.Streams.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            repository.GetOrAddNew(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => result = repository.AddFailingPartition(stream_processor_id, partition, failing_partition_position, failing_partition_retry_time, reason, CancellationToken.None).GetAwaiter().GetResult();

        It should_have_the_same_position = () => result.Position.ShouldEqual(initial_state.Position);
        It should_have_one_failing_partition = () => result.FailingPartitions.Count.ShouldEqual(1);
        It should_have_one_failing_partition_with_the_correct_partition_id = () => result.FailingPartitions.TryGetValue(partition, out var state).ShouldBeTrue();
        It should_have_one_failing_partition_with_the_correct_position = () => result.FailingPartitions[partition].Position.ShouldEqual(failing_partition_position);
        It should_have_one_failing_partition_with_the_correct_retry_time = () => result.FailingPartitions[partition].RetryTime.ShouldEqual(failing_partition_retry_time);
        It should_have_one_failing_partition_with_the_correct_reason = () => result.FailingPartitions[partition].Reason.ShouldEqual(reason);
    }
}