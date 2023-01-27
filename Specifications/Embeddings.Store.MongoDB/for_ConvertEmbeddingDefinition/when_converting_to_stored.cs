// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.State;
using FluentAssertions;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.for_ConvertEmbeddingDefinition;

public class when_converting_to_stored
{

    static EmbeddingId embedding;
    static IEnumerable<Artifact> event_types;
    static ProjectionState initial_state;
    static Store.Definition.EmbeddingDefinition runtime_definition;

    static IConvertEmbeddingDefinition converter;

    Establish context = () =>
    {
        embedding = new EmbeddingId(Guid.Parse("2174fa82-de09-4ce5-a411-433203a29370"));
        event_types = new List<Artifact>
        {
            new("7dd78147-14fc-4a4c-a42e-2d81bd73a4a4", 0),
            new("c98f03ee-7f44-456e-9f56-07ba5a73764c", 3)
        };
        dynamic json = new JObject();
        json.FirstProp = "FirstProp";
        json.SecondProp = "the 👑 cooler ❄❄ second 👆 prop";
        initial_state = new(JsonConvert.SerializeObject(json));
        runtime_definition = new Store.Definition.EmbeddingDefinition(embedding, event_types, initial_state);
        converter = new ConvertEmbeddingDefinition();
    };

    static EmbeddingDefinition result_definition;

    Because of = () => result_definition = converter.ToStored(runtime_definition);

    It should_have_the_embedding = () => result_definition.Embedding.Should().Be(embedding.Value);
    It should_have_the_event_selectors = () => result_definition.Events.ShouldContainOnly(event_types.Select(_ => _.Id.Value));
    It should_have_the_state = () => result_definition.InitialState.Should().Be(initial_state.Value);
}