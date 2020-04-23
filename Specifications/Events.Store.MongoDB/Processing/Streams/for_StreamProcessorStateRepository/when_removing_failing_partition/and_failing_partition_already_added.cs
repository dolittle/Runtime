// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.for_StreamProcessorStateRepository.when_removing_failing_partition
{
    public class and_failing_partition_already_added : given.all_dependencies
    {
        static StreamProcessorStateRepository repository;
        static PartitionId partition;
        static Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id;
        static Runtime.Events.Processing.Streams.StreamProcessorState initial_state;
        static Runtime.Events.Processing.Streams.StreamProcessorState result;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            initial_state = Runtime.Events.Processing.Streams.StreamProcessorState.New;
            partition = Guid.NewGuid();
            stream_processor_id = new Runtime.Events.Processing.Streams.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            repository.GetOrAddNew(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult();
            repository.AddFailingPartition(stream_processor_id, partition, 0U, DateTimeOffset.UtcNow, "reason", CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => result = repository.RemoveFailingPartition(stream_processor_id, partition, CancellationToken.None).GetAwaiter().GetResult();

        It should_have_the_same_position = () => result.Position.ShouldEqual(initial_state.Position);
        It should_not_have_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(0);
    }
}