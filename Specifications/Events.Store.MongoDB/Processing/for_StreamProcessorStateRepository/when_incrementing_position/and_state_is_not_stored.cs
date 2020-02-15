// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_StreamProcessorStateRepository.when_incrementing_position
{
    public class and_state_is_not_stored : given.all_dependencies
    {
        static Runtime.Events.Processing.Streams.StreamProcessorState initial_state;
        static StreamProcessorStateRepository repository;
        static Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id;
        static Exception exception;

        Establish context = () =>
        {
            repository = new StreamProcessorStateRepository(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_processor_id = new Runtime.Events.Processing.Streams.StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
            initial_state = Runtime.Events.Processing.Streams.StreamProcessorState.New;
        };

        Because of = () => exception = Catch.Exception(() => repository.IncrementPosition(stream_processor_id).GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_it_could_not_find_stream_processor = () => exception.ShouldBeOfExactType<Runtime.Events.Processing.Streams.StreamProcessorNotFound>();
    }
}