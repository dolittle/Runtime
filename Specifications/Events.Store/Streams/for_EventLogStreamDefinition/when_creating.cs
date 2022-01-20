// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventLogStreamDefinition;

public class when_creating
{
    static EventLogStreamDefinition event_log_stream_definition;

    Because of = () => event_log_stream_definition = new EventLogStreamDefinition();

    It should_not_be_partitioned = () => event_log_stream_definition.Partitioned.ShouldBeFalse();
    It should_not_be_public = () => event_log_stream_definition.Public.ShouldBeFalse();
    It should_have_the_event_log_stream_id = () => event_log_stream_definition.StreamId.ShouldEqual(StreamId.EventLog);
}