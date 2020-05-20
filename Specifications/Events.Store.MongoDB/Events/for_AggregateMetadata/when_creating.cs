// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_AggregateMetadata
{
    public class when_creating
    {
        static bool was_applied_by_aggregate;
        static Guid type_id;
        static uint type_generation;
        static ulong version;
        static AggregateMetadata result;

        Establish context = () =>
        {
            was_applied_by_aggregate = true;
            type_id = Guid.NewGuid();
            type_generation = (uint)random.natural_number;
            version = (ulong)random.natural_number;
        };

        Because of = () => result = new AggregateMetadata(
            was_applied_by_aggregate,
            type_id,
            type_generation,
            version);

        It should_have_the_correct_was_applied_by_aggregate_value = () => result.WasAppliedByAggregate.ShouldEqual(was_applied_by_aggregate);
        It should_have_the_correct_type_id = () => result.TypeId.ShouldEqual(type_id);
        It should_have_the_correct_type_generation = () => result.TypeGeneration.ShouldEqual(type_generation);
        It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
    }
}