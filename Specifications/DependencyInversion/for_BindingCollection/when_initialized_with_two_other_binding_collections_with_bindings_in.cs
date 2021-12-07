// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingCollection;

public class when_initialized_with_two_other_binding_collections_with_bindings_in
{
    static BindingCollection collection;

    Because of = () => collection = new BindingCollection(
        new[]
        {
            new Binding(typeof(string), new Strategies.Null(), new Scopes.Transient())
        },
        new[]
        {
            new Binding(typeof(object), new Strategies.Null(), new Scopes.Transient())
        });

    It should_have_first_binding_from_the_first_collection = () => collection.First().Service.ShouldEqual(typeof(string));
    It should_have_first_binding_from_the_second_collection = () => collection.ToArray()[1].Service.ShouldEqual(typeof(object));
}