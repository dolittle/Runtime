// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Lifecycle;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder
{
    public class when_binding_to_type_that_indicates_singleton_per_tenant_lifecycle_as_attribte : given.a_null_binding
    {
        [SingletonPerTenant]
        class MyType { }

        static Binding result;

        Because of = () =>
        {
            builder.To(typeof(MyType));
            result = builder.Build();
        };

        It should_have_a_type_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.Type>();
        It should_hold_the_type_in_the_strategy = () => ((Strategies.Type)result.Strategy).Target.ShouldEqual(typeof(MyType));
        It should_have_singleton_per_tenant_scope = () => result.Scope.ShouldBeAssignableTo<Scopes.SingletonPerTenant>();
    }
}