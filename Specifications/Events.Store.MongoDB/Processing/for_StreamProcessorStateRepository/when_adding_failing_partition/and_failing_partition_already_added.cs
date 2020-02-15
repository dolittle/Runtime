// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_StreamProcessorStateRepository.when_adding_failing_partition
{
    public class and_failing_partition_already_added : given.all_dependencies
    {
        static StreamProcessorStateRepository repository;
        static Runtime.Events.Processing.PartitionId partition;
        static Runtime.Events.Processing.StreamProcessorId stream_processor_id;
        static Exception exception;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            partition = Guid.NewGuid();
            stream_processor_id = new Runtime.Events.Processing.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
            repository.GetOrAddNew(stream_processor_id).GetAwaiter().GetResult();
            repository.AddFailingPartition(stream_processor_id, partition, 0U, DateTimeOffset.UtcNow, "").GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => repository.AddFailingPartition(stream_processor_id, partition, 0U, DateTimeOffset.UtcNow, "").GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_failing_partition_has_already_been_added = () => exception.ShouldBeOfExactType<Runtime.Events.Processing.FailingPartitionAlreadyExists>();
    }
}