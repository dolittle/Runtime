// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB.for_CollectionName.when_constructing;

public class and_name_contains_dollar_character
{
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => new CollectionName("almost_legal_name_$"));

    It should_fail = () => exception.Should().BeOfType<InvalidMongoDBCollectionName>();
}