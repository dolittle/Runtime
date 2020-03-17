// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Dolittle.Types.Testing;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static Mock<EventTypesFromEventLogFetcher> event_types_from_event_log_fetcher;
        protected static Mock<PublicEventTypesFetcher> public_event_types_fetcher;
        protected static EventTypesFromStreamsFetcher event_types_from_streams;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            event_types_from_event_log_fetcher = new Mock<EventTypesFromEventLogFetcher>(an_event_store_connection, Mock.Of<ILogger>());
            public_event_types_fetcher = new Mock<PublicEventTypesFetcher>(an_event_store_connection, Mock.Of<ILogger>());
            event_types_from_streams = new EventTypesFromStreamsFetcher(
                new StaticInstancesOf<ICanFetchEventTypesFromWellKnownStreams>(event_types_from_event_log_fetcher.Object, public_event_types_fetcher.Object),
                an_event_store_connection,
                Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}