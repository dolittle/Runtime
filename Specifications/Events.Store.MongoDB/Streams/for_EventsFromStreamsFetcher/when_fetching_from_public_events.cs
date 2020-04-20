// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher
{
    public class when_fetching_from_public_events : given.all_dependencies
    {
        Because of = () => events_from_streams_fetcher.Fetch(StreamId.PublicEventsId, 0U).GetAwaiter().GetResult();

        It should_not_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_use_public_events_fetcher = () => public_events_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Once);
    }
}