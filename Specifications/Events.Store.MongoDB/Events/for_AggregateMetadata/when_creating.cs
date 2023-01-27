// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_AggregateMetadata;

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
        type_id = Guid.Parse("2fbf871b-a5e2-4600-8e4f-563c1a200cf0");
        type_generation = 2901939569;
        version = 1670515463;
    };

    Because of = () => result = new AggregateMetadata(
        was_applied_by_aggregate,
        type_id,
        type_generation,
        version);

    It should_have_the_correct_was_applied_by_aggregate_value = () => result.WasAppliedByAggregate.Should().Be(was_applied_by_aggregate);
    It should_have_the_correct_type_id = () => result.TypeId.Should().Be(type_id);
    It should_have_the_correct_type_generation = () => result.TypeGeneration.Should().Be(type_generation);
    It should_have_the_correct_version = () => result.Version.Should().Be(version);
}