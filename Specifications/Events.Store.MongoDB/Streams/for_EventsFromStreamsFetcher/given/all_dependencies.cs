// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Dolittle.Types.Testing;
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

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            events_from_event_log_fetcher = new Mock<EventFromEventLogFetcher>(an_event_store_connection, Mock.Of<ILogger>());
            public_events_fetcher = new Mock<PublicEventsFetcher>(an_event_store_connection, Mock.Of<ILogger>());

            events_from_streams_fetcher = new EventsFromStreamsFetcher(
                new StaticInstancesOf<ICanFetchEventsFromWellKnownStreams>(events_from_event_log_fetcher.Object, public_events_fetcher.Object),
                an_event_store_connection,
                Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}