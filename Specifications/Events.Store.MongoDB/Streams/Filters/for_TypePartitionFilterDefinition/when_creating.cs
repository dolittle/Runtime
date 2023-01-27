// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters.for_TypePartitionFilterDefinition;

public class when_creating
{
    static IEnumerable<Guid> types;
    static TypePartitionFilterDefinition result;

    Establish context = () =>
    {
        types = new[]
        {
            Guid.Parse("f0d8f3b0-9e59-4879-93da-d1e230d88493"),
            Guid.Parse("19192073-b0ba-46a9-a1cd-f75bfc9bcd20"),
            Guid.Parse("a423f803-3c73-49eb-a517-78b0311b1b00")
        };
    };

    Because of = () => result = new TypePartitionFilterDefinition(types);

    It should_have_the_correct_types = () => result.Types.ShouldContainOnly(types);
}