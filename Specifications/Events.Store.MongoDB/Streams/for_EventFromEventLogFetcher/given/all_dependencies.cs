// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static EventFromEventLogFetcher fetcher;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            fetcher = new EventFromEventLogFetcher(an_event_store_connection, Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}