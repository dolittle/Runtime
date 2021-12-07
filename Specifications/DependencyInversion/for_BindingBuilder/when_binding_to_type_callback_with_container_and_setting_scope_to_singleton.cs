// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder;

public class when_binding_to_type_callback_with_container_and_setting_scope_to_singleton : given.a_null_binding
{
    static Binding result;
    static Func<Type> callback = () => typeof(string);

    Because of = () =>
    {
        builder.To(callback).Singleton();
        result = builder.Build();
    };

    It should_have_a_type_callback_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.TypeCallback>();
    It should_hold_the_delegeate_in_the_strategy = () => ((Strategies.TypeCallback)result.Strategy).Target.ShouldEqual(callback);
    It should_have_singleton_scope = () => result.Scope.ShouldBeAssignableTo<Scopes.Singleton>();
}