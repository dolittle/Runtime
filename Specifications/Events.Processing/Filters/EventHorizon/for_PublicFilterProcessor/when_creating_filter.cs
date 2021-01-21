// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon.for_PublicFilterProcessor
{
    public class when_creating_filter : given.a_public_event_filter
    {
        static PublicFilterDefinition filter_definition;

        Establish context = () =>
        {
            filter_definition = new PublicFilterDefinition(Guid.NewGuid(), Guid.NewGuid());
        };

        Because of = () => filter = new PublicFilterProcessor(
            filter_definition,
            dispatcher.Object,
            Moq.Mock.Of<IWriteEventsToPublicStreams>(),
            Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => filter.Identifier.Value.ShouldEqual(filter_definition.TargetStream.Value);
        It should_have_the_correct_source_stream = () => filter.Definition.SourceStream.ShouldEqual(filter_definition.SourceStream);
        It should_have_the_correct_target_stream = () => filter.Definition.TargetStream.ShouldEqual(filter_definition.TargetStream);
    }
}