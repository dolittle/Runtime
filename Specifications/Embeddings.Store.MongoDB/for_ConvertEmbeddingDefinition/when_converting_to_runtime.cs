// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Embeddings.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EmbeddingDefinition = Dolittle.Runtime.Embeddings.Store.Definition.EmbeddingDefinition;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.for_ConvertEmbeddingDefinition
{
    public class when_converting_to_runtime
    {
        static EmbeddingId embedding;
        static ProjectionEventSelector[] event_selectors;
        static ProjectionState initial_state;
        static Definition.EmbeddingDefinition stored_definition;
        static IConvertEmbeddingDefinition converter;

        Establish context = () =>
        {
            embedding = new EmbeddingId(Guid.Parse("2174fa82-de09-4ce5-a411-433203a29370"));
            var selector_list = new List<ProjectionEventSelector>()
            {
                new ProjectionEventSelector
                {
                    EventType = Guid.Parse("7dd78147-14fc-4a4c-a42e-2d81bd73a4a4"),
                    EventKeySelectorType = 0,
                    EventKeySelectorExpression = "event 👐👐 key 🔑🔑 selector expression"
                },
                new ProjectionEventSelector
                {
                    EventType = Guid.Parse("c98f03ee-7f44-456e-9f56-07ba5a73764c"),
                    EventKeySelectorType = 0,
                    EventKeySelectorExpression = "another 🔁 event 👐👐 key 🔑🔑 🔑🔑 selector expression"
                }
            };
            event_selectors = selector_list.ToArray();
            dynamic json = new JObject();
            json.FirstProp = "crazy prop";
            json.SecondProp = "the 🎇🅱 craziest property";
            initial_state = new(JsonConvert.SerializeObject(json));
            stored_definition = new Definition.EmbeddingDefinition
            {
                Embedding = embedding,
                InitialState = BsonDocument.Parse(initial_state),
                InitialStateRaw = initial_state,
                EventSelectors = event_selectors
            };
            converter = new ConvertEmbeddingDefinition();
        };

        static EmbeddingDefinition result_definition;

        Because of = () => result_definition = converter.ToRuntime(stored_definition);

        It should_have_the_embedding = () => result_definition.Embedding.ShouldEqual(embedding);
        It should_have_the_event_selectors = () => result_definition.Events
            .ShouldEachConformTo(_ =>
                event_selectors.Any(selector =>
                    selector.EventType == _.EventType.Value
                    && selector.EventKeySelectorType == _.KeySelectorType
                    && selector.EventKeySelectorExpression == _.KeySelectorExpression
                ));
        It should_have_the_state = () => result_definition.InititalState.ShouldEqual(initial_state);
    }
}
