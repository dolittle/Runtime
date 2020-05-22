// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventMetadata
{
    public class when_creating
    {
        static DateTime occurred;
        static Guid event_source;
        static Guid type_id;
        static uint type_generation;
        static bool is_public;
        static EventMetadata result;

        Establish context = () =>
        {
            occurred = new DateTime(2852152428, DateTimeKind.Utc);
            event_source = Guid.Parse("bfbfb1aa-b6c1-4bc3-9269-23d5e4205231");
            type_id = Guid.Parse("5b92f887-a986-4cde-a375-cb868c50eee2");
            type_generation = 678275961;
            is_public = true;
        };

        Because of = () => result = new EventMetadata(
            occurred,
            event_source,
            type_id,
            type_generation,
            is_public);

        It should_have_the_correct_occurred_value = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_type_id = () => result.TypeId.ShouldEqual(type_id);
        It should_have_the_correct_type_generation = () => result.TypeGeneration.ShouldEqual(type_generation);
        It should_have_the_correct_public_value = () => result.Public.ShouldEqual(is_public);
    }
}