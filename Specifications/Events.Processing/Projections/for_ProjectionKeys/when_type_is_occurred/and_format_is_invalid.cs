// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.when_type_is_occurred;

public class and_format_is_invalid : all_dependencies
{
    static ProjectionDefinition definition;
    static CommittedEvent committed_event;
    static PartitionId partition;
    static bool result;
    static OccurredFormat occurred_format;
    static ProjectionKey key;

    Establish context = () =>
    {
        occurred_format = "Ø";
        committed_event = given.an_event.that_occurred_at(DateTimeOffset.Parse("05/01/2020 12:32:31"));
        partition = "/(partition!";
        definition = projection_definition_builder.create()
            .with_selector(ProjectionEventSelector.Occurred(committed_event.Type.Id, occurred_format))
            .Build();
    };
    Because of = () => result = projection_keys.TryGetFor(definition, committed_event, partition, out key);

    It should_not_get_key = () => result.Should().BeFalse();
}