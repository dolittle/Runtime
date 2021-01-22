// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder
{
    public class when_binding_to_callback_using_generic_service : given.a_null_binding_for_generic_builder
    {
        const string return_value = "Fourty Two";
        static Binding result;
        static Func<string> callback = () => return_value;

        Because of = () =>
        {
            builder.To(callback);
            result = builder.Build();
        };

        It should_have_a_callback_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.Callback<string>>();
        It should_forward_to_given_callback_when_called = () => ((Strategies.Callback)result.Strategy).Target().ShouldEqual(return_value);
        It should_have_transient_scope = () => result.Scope.ShouldBeAssignableTo<Scopes.Transient>();
    }
}