// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Producer.for_PublicEventFilterDefinition
{
    public class when_creating_definition
    {
        static PublicEventFilterDefinition definition;

        Because of = () => definition = new PublicEventFilterDefinition();

        It should_have_event_log_as_source_stream = () => definition.SourceStream.ShouldEqual(StreamId.AllStreamId);
        It should_have_the_correct_target_stream = () => definition.TargetStream.ShouldEqual(StreamId.PublicEventsId);
        It should_have_the_correct_stream_name = () => definition.StreamName.ShouldEqual(PublicEventFilterDefinition.PublicEventsStreamName);
    }
}