// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder
{
    public class when_binding_to_callback_with_binding_context : given.a_null_binding
    {
        static Binding result;
        static Func<BindingContext, object> callback = (bindingContext) => "result";

        Because of = () =>
        {
            builder.To(callback);
            result = builder.Build();
        };

        It should_have_a_callback_strategy = () => result.Strategy.ShouldBeOfExactType<Strategies.CallbackWithBindingContext>();
        It should_hold_the_delegate_in_the_strategy = () => ((Strategies.CallbackWithBindingContext)result.Strategy).Target.ShouldEqual(callback);
        It should_have_transient_scope = () => result.Scope.ShouldBeAssignableTo<Scopes.Transient>();
    }
}