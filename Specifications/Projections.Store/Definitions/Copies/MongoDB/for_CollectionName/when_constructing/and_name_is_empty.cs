// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB.for_CollectionName.when_constructing;

public class and_name_is_empty
{
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => new CollectionName(""));

    It should_fail = () => exception.ShouldBeOfExactType<InvalidMongoDBCollectionName>();
}