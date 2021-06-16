// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder
{
    public class when_binding_to_generic_type_using_generic_service : given.a_null_binding_for_generic_builder
    {
        static Binding result;

        Because of = () =>
        {
            builder.To<string>();
            result = builder.Build();
        };

        It should_have_a_type_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.Type>();
        It should_hold_the_type_in_the_strategy = () => ((Strategies.Type)result.Strategy).Target.ShouldEqual(typeof(string));
    }
}