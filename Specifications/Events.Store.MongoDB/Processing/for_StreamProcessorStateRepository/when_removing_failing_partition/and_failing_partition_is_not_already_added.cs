// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_StreamProcessorStateRepository.when_removing_failing_partition
{
    public class and_failing_partition_is_not_already_added : given.all_dependencies
    {
        static StreamProcessorStateRepository repository;
        static Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id;
        static Exception exception;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_processor_id = new Runtime.Events.Processing.Streams.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
            repository.GetOrAddNew(stream_processor_id).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => repository.RemoveFailingPartition(stream_processor_id, Guid.NewGuid()).GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_failing_partition_does_not_exist = () => exception.ShouldBeOfExactType<Runtime.Events.Processing.Streams.FailingPartitionDoesNotExist>();
    }
}