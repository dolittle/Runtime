// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher.when_finding_next_position.in_a_stream
{
    public class with_no_events : given.all_dependencies
    {
        static StreamPosition result;

        Because of = () => result = events_from_streams_fetcher.FindNext(Guid.NewGuid(), Guid.NewGuid(), 0U).GetAwaiter().GetResult();

        It should_return_the_max_value_of_uint = () => result.Value.ShouldEqual(uint.MaxValue);
    }
}