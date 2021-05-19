// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.for_ConvertEmbeddingDefinition
{
    public class when_converting_to_stored
    {

        static EmbeddingId embedding;
        static IEnumerable<ProjectionEventSelector> event_selectors;
        static ProjectionState initial_state;
        static Store.Definition.EmbeddingDefinition runtime_definition;

        static IConvertEmbeddingDefinition converter;

        Establish context = () =>
        {
            embedding = new EmbeddingId(Guid.Parse("2174fa82-de09-4ce5-a411-433203a29370"));
            event_selectors = new List<ProjectionEventSelector>
            {
                new ProjectionEventSelector(
                    Guid.Parse("7dd78147-14fc-4a4c-a42e-2d81bd73a4a4"),
                    0,
                    "event 👐👐 key 🔑🔑 selector expression"
                ),
                new ProjectionEventSelector(
                    Guid.Parse("c98f03ee-7f44-456e-9f56-07ba5a73764c"),
                    0,
                    "another 🔁 event 👐👐 key 🔑🔑 🔑🔑 selector expression"
                )
            };
            dynamic json = new JObject();
            json.FirstProp = "FirstProp";
            json.SecondProp = "the 👑 cooler ❄❄ second 👆 prop";
            initial_state = new(JsonConvert.SerializeObject(json));
            runtime_definition = new(embedding, event_selectors, initial_state);
            converter = new ConvertEmbeddingDefinition();
        };

        static EmbeddingDefinition result_definition;

        Because of = () => result_definition = converter.ToStored(runtime_definition);

        It should_have_the_embedding = () => result_definition.Embedding.ShouldEqual(embedding.Value);
        It should_have_the_event_selectors = () => result_definition.EventSelectors
            .ShouldEachConformTo(_ => event_selectors
                .Contains(new ProjectionEventSelector(_.EventType, _.EventKeySelectorType, _.EventKeySelectorExpression)));
        It should_have_the_raw_state = () => result_definition.InitialStateRaw.ShouldEqual(initial_state.Value);
    }
}
