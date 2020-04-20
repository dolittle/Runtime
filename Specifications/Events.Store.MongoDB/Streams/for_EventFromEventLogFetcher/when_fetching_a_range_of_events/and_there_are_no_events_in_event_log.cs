// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.when_fetching_a_range_of_events
{
    public class and_there_are_no_events_in_event_log : given.all_dependencies
    {
        static IEnumerable<Runtime.Events.Streams.StreamEvent> result;

        Because of = () => result = fetcher.FetchRange(StreamId.AllStreamId, new StreamPositionRange(0, 1)).GetAwaiter().GetResult();

        It should_return_an_empty_list = () => result.ShouldBeEmpty();
    }
}