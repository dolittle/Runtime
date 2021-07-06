// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Store.for_ProjectionKeyToEventSourceIdConverter.when_getting_event_source_id_for
{
    public class two_values_interleaved : given.a_converter
    {
        static ProjectionKey projection_key_one;
        static ProjectionKey projection_key_two;

        Establish context = () =>
        {
            projection_key_one = "8 Ralle Boulevard";
            projection_key_two = "1811 Buan Key";
        };

        static EventSourceId event_source_id_one;
        static EventSourceId event_source_id_two;
        static EventSourceId event_source_id_three;
        static EventSourceId event_source_id_four;
        static EventSourceId event_source_id_five;

        Because of = () =>
        {
            event_source_id_one = converter.GetEventSourceIdFor(projection_key_one);
            event_source_id_two = converter.GetEventSourceIdFor(projection_key_two);
            event_source_id_three = converter.GetEventSourceIdFor(projection_key_one);
            event_source_id_four = converter.GetEventSourceIdFor(projection_key_one);
            event_source_id_five = converter.GetEventSourceIdFor(projection_key_two);
        };

        It should_return_the_first_id_the_third_time = () => event_source_id_three.ShouldEqual(event_source_id_one);
        It should_return_the_first_id_the_fourth_time = () => event_source_id_four.ShouldEqual(event_source_id_one);
        It should_return_the_second_id_the_fifth_time = () => event_source_id_five.ShouldEqual(event_source_id_two);
    }
}
