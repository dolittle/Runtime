// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range
{
    public class and_from_position_is_greater_than_to_position : given.all_dependencies
    {
        static StreamId stream;
        static Exception exception;

        Establish context = () => stream = Guid.NewGuid();

        Because of = () => exception = Catch.Exception(() => event_types_from_streams.FetchTypesInRange(stream, new StreamPositionRange(1U, 0U)).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_from_position_is_greater_than_to = () => exception.ShouldBeOfExactType<FromPositionIsGreaterThanToPosition>();
    }
}