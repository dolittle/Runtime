// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB.for_CollectionName.when_constructing;

public class and_name_starts_with_system_prefix
{
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => new CollectionName("system.almost_legal_name"));

    It should_fail = () => exception.ShouldBeOfExactType<InvalidMongoDBCollectionName>();
}