// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder
{
    public class when_binding_to_type_callback_with_binding_context_using_generic_service : given.a_null_binding_for_generic_builder
    {
        static Type return_value = typeof(string);
        static Binding result;
        static BindingContext expected_binding_context;
        static BindingContext binding_context;

        static Func<BindingContext, Type> callback = (c) =>
        {
            binding_context = c;
            return return_value;
        };

        Because of = () =>
        {
            expected_binding_context = new BindingContext(typeof(object));
            builder.To(callback);
            result = builder.Build();
        };

        It should_have_a_type_callback_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.TypeCallbackWithBindingContext>();
        It should_forward_to_given_callback_when_called = () => ((Strategies.TypeCallbackWithBindingContext)result.Strategy).Target(expected_binding_context).ShouldEqual(return_value);
        It should_have_transient_scope = () => result.Scope.ShouldBeAssignableTo<Scopes.Transient>();
        It should_forward_binding_context = () => binding_context.ShouldEqual(expected_binding_context);
    }
}