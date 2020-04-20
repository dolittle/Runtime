// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.when_finding_next_position
{
    public class and_there_are_no_events_in_event_log : given.all_dependencies
    {
        static StreamPosition result;

        Because of = () => result = fetcher.FindNext(StreamId.AllStreamId, PartitionId.NotSet, 0).GetAwaiter().GetResult();

        It should_return_the_max_value = () => result.Value.ShouldEqual(uint.MaxValue);
    }
}