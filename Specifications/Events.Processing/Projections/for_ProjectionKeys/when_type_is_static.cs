// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys;

public class when_type_is_static : given.all_dependencies
{
    static ProjectionDefinition definition;
    static CommittedEvent committed_event;
    static PartitionId partition;
    static bool result;
    static ProjectionKey static_key;
    static ProjectionKey key;

    Establish context = () =>
    {
        static_key = "some key";
        committed_event = committed_events.single("{\"property\": \"value\"}");
        partition = "/(partition!";
        definition = given.projection_definition_builder.create()
            .with_selector(ProjectionEventSelector.Static(committed_event.Type.Id, static_key))
            .Build();
    };
    Because of = () => result = projection_keys.TryGetFor(definition, committed_event, partition, out key);

    It should_get_key = () => result.ShouldBeTrue();
    It should_have_key_be_the_static_key = () => key.ShouldEqual(static_key);
}