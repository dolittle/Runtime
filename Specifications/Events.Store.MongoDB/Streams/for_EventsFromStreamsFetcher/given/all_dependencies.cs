// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static Mock<EventFromEventLogFetcher> events_from_event_log_fetcher;
        protected static Mock<PublicEventsFetcher> public_events_fetcher;
        protected static EventsFromStreamsFetcher events_from_streams_fetcher;

        static IEnumerable<ICanFetchEventsFromWellKnownStreams> fetchers;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            events_from_event_log_fetcher = new Mock<EventFromEventLogFetcher>();
            public_events_fetcher = new Mock<PublicEventsFetcher>();
            events_from_event_log_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<Runtime.Events.Streams.StreamEvent>(null));
            public_events_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<Runtime.Events.Streams.StreamEvent>(null));
            fetchers = new ICanFetchEventsFromWellKnownStreams[] { events_from_event_log_fetcher.Object, public_events_fetcher.Object };

            var instances_of_fetchers = new Mock<IInstancesOf<ICanFetchEventsFromWellKnownStreams>>();
            instances_of_fetchers.Setup(_ => _.GetEnumerator()).Returns(fetchers.GetEnumerator());

            events_from_streams_fetcher = new EventsFromStreamsFetcher(
                instances_of_fetchers.Object,
                an_event_store_connection,
                Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}