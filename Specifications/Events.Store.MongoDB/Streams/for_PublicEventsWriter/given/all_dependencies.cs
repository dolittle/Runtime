// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static PublicEventsWriter public_events_writer;

        static IEnumerable<ICanWriteEventsToWellKnownStreams> writers;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            public_events_writer = new PublicEventsWriter(an_event_store_connection, Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}