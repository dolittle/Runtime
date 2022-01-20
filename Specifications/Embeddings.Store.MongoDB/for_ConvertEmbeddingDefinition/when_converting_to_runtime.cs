// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EmbeddingDefinition = Dolittle.Runtime.Embeddings.Store.Definition.EmbeddingDefinition;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.for_ConvertEmbeddingDefinition;

public class when_converting_to_runtime
{
    static EmbeddingId embedding;
    static Guid[] event_types;
    static ProjectionState initial_state;
    static Definition.EmbeddingDefinition stored_definition;
    static IConvertEmbeddingDefinition converter;

    Establish context = () =>
    {
        embedding = new EmbeddingId(Guid.Parse("2174fa82-de09-4ce5-a411-433203a29370"));
        event_types = new[]
        {
            Guid.Parse("7dd78147-14fc-4a4c-a42e-2d81bd73a4a4"),
            Guid.Parse("c98f03ee-7f44-456e-9f56-07ba5a73764c"),
        };
        dynamic json = new JObject();
        json.FirstProp = "crazy prop";
        json.SecondProp = "the 🎇🅱 craziest property";
        initial_state = new(JsonConvert.SerializeObject(json));
        stored_definition = new Definition.EmbeddingDefinition
        {
            Embedding = embedding,
            InitialState = initial_state,
            Events = event_types
        };
        converter = new ConvertEmbeddingDefinition();
    };

    static EmbeddingDefinition result_definition;

    Because of = () => result_definition = converter.ToRuntime(stored_definition);

    It should_have_the_embedding = () => result_definition.Embedding.ShouldEqual(embedding);
    It should_have_the_event_types = () => result_definition.Events.ShouldContainOnly(event_types.Select(_ => new Artifact(_, ArtifactGeneration.First)));
    It should_have_the_state = () => result_definition.InititalState.ShouldEqual(initial_state);
}