// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys;

public class when_type_is_partition : given.all_dependencies
{
    static ProjectionDefinition definition;
    static CommittedEvent committed_event;
    static PartitionId partition;
    static bool result;
    static ProjectionKey key;

    Establish context = () =>
    {
        committed_event = committed_events.single();
        partition = "___partition__";
        definition = given.projection_definition_builder.create()
            .with_selector(ProjectionEventSelector.PartitionId(committed_event.Type.Id))
            .Build();
    };
    Because of = () => result = projection_keys.TryGetFor(definition, committed_event, partition, out key);

    It should_get_key = () => result.Should().BeTrue();
    It should_have_the_correct_key = () => key.Value.Should().Be(partition.Value.ToString());
}