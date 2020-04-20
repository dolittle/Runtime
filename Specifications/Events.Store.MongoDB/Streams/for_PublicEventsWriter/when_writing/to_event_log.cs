// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_writing
{
    public class to_event_log : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => public_events_writer.Write(committed_events.a_committed_event(0), StreamId.AllStreamId, PartitionId.NotSet).GetAwaiter().GetResult());

        It should_fail_because_it_cannot_write_event_to_all_stream = () => exception.ShouldBeOfExactType<EventsToWellKnownStreamsWriterCannotWriteToStream>();
    }
}