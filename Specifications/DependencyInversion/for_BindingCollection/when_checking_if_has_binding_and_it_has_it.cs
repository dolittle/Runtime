// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingCollection
{
    public class when_checking_if_has_binding_and_it_has_it
    {
        static BindingCollection collection;
        static bool result;

        Establish context = () => collection = new BindingCollection(new[] { new Binding(typeof(string), new Strategies.Null(), new Scopes.Transient()) });

        Because of = () => result = collection.HasBindingFor<string>();

        It should_have_it = () => result.ShouldBeTrue();
    }
}