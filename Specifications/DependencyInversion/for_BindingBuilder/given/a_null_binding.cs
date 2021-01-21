// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder.given
{
    public class a_null_binding
    {
        protected static Binding binding;
        protected static BindingBuilder builder;

        Establish context = () =>
        {
            binding = new Binding(typeof(object), new Strategies.Null(), new Scopes.Transient());
            builder = new BindingBuilder(binding);
        };
    }
}