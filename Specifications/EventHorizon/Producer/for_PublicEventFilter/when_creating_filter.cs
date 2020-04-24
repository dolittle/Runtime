// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Producer.for_PublicEventFilter
{
    public class when_creating_filter
    {
        static PublicEventFilter filter;

        Because of = () => filter = new PublicEventFilter(Moq.Mock.Of<IWriteEventsToStreams>(), Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => filter.Identifier.Value.ShouldEqual(StreamId.PublicEventsId.Value);
        It should_have_the_correct_source_stream = () => filter.Definition.SourceStream.ShouldEqual(StreamId.AllStreamId);
        It should_have_the_correct_target_stream = () => filter.Definition.TargetStream.ShouldEqual(StreamId.PublicEventsId);
    }
}