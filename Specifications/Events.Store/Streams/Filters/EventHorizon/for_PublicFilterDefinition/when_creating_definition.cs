// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon.for_PublicFilterDefinition;

public class when_creating_definition
{
    static PublicFilterDefinition definition;
    static StreamId source_stream;
    static StreamId target_stream;

    Establish context = () =>
    {
        source_stream = Guid.NewGuid();
        target_stream = Guid.NewGuid();
    };

    Because of = () => definition = new PublicFilterDefinition(source_stream, target_stream);

    It should_have_event_log_as_source_stream = () => definition.SourceStream.Should().Be(source_stream);
    It should_have_the_correct_target_stream = () => definition.TargetStream.Should().Be(target_stream);
}