// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range_and_partition
{
    public class and_from_position_is_greater_than_to_position : given.all_dependencies
    {
        static EventTypesFromStreamsFetcher event_types_from_streams;
        static StreamId stream;
        static Exception exception;

        Establish context = () =>
        {
            event_types_from_streams = new EventTypesFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
        };

        Because of = () => exception = Catch.Exception(() => event_types_from_streams.FetchTypesInRange(stream, 1U, 0U).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_from_position_is_greater_than_to = () => exception.ShouldBeOfExactType<InvalidStreamPositionRange>();
    }
}