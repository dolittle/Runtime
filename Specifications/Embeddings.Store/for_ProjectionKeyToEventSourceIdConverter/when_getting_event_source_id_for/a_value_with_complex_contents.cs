// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Store.for_ProjectionKeyToEventSourceIdConverter.when_getting_event_source_id_for
{
    public class a_value_with_complex_contents : given.a_converter
    {
        static ProjectionKey projection_key;

        Establish context = () => projection_key = @"
            {
                ""here"": ""is"",
                ""some"": ""json content with emojis 👩‍🚀 🚀"",
                ""a number"": 1337,
                ""and a bool"": true
            }";

        static EventSourceId event_source_id;

        Because of = () => event_source_id = converter.GetEventSourceIdFor(projection_key);

        It should_return_a_special_event_source = () => event_source_id.Value.ShouldEqual(projection_key.Value);
    }
}
