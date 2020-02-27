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

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsToStreamsWriter.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static Mock<PublicEventsWriter> public_events_writer;
        protected static EventsToStreamsWriter events_to_streams_writer;

        static IEnumerable<ICanWriteEventsToWellKnownStreams> writers;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            public_events_writer = new Mock<PublicEventsWriter>();
            public_events_writer.Setup(_ => _.Write(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            writers = new ICanWriteEventsToWellKnownStreams[] { public_events_writer.Object };

            var instances_of_writers = new Mock<IInstancesOf<ICanWriteEventsToWellKnownStreams>>();
            instances_of_writers.Setup(_ => _.GetEnumerator()).Returns(writers.GetEnumerator());
            events_to_streams_writer = new EventsToStreamsWriter(instances_of_writers.Object, an_event_store_connection, Mock.Of<ILogger>());
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}