// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.for_StreamProcessorStateRepository.when_getting_state
{
    public class and_state_is_not_stored : given.all_dependencies
    {
        static Runtime.Events.Processing.Streams.StreamProcessorState initial_state;
        static StreamProcessorStateRepository repository;
        static Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id;
        static Runtime.Events.Processing.Streams.StreamProcessorState result;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_processor_id = new Runtime.Events.Processing.Streams.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
            initial_state = Runtime.Events.Processing.Streams.StreamProcessorState.New;
        };

        Because of = () => result = repository.GetOrAddNew(stream_processor_id).GetAwaiter().GetResult();

        It should_have_the_same_position = () => result.Position.Value.ShouldEqual(initial_state.Position.Value);
        It should_have_the_same_number_of_failing_partitions = () => result.FailingPartitions.Count.ShouldEqual(initial_state.FailingPartitions.Count);
    }
}