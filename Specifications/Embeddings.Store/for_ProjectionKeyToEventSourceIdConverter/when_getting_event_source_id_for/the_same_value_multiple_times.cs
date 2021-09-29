// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Store.for_ProjectionKeyToEventSourceIdConverter.when_getting_event_source_id_for
{
    public class the_same_value_multiple_times : given.a_converter
    {
        static ProjectionKey projection_key;

        Establish context = () => projection_key = "Estella Henry";

        static EventSourceId event_source_id_one;
        static EventSourceId event_source_id_two;
        static EventSourceId event_source_id_three;

        Because of = () =>
        {
            event_source_id_one = converter.GetEventSourceIdFor(projection_key);
            event_source_id_two = converter.GetEventSourceIdFor(projection_key);
            event_source_id_three = converter.GetEventSourceIdFor(projection_key);
        };

        It should_return_the_same_id_the_second_time = () => event_source_id_two.ShouldEqual(event_source_id_one);
        It should_return_the_same_id_the_third_time = () => event_source_id_three.ShouldEqual(event_source_id_one);
    }
}
