// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB.for_CollectionName.when_constructing;

public class and_name_is_valid
{
    static CollectionName collection_name;

    Because of = () => collection_name = new CollectionName("legal_name");

    It should_create_the_concept_with_the_correct_value = () => collection_name.Value.Should().Be("legal_name");
}