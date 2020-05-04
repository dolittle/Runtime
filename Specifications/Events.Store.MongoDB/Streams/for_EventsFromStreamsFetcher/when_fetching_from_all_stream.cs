// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher
{
    public class when_fetching_from_all_stream : given.all_dependencies
    {
        Because of = () => events_from_streams_fetcher.Fetch(ScopeId.Default, StreamId.AllStreamId, 0U, CancellationToken.None).GetAwaiter().GetResult();

        It should_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}