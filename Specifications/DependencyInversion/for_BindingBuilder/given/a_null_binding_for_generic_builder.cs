// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingBuilder.given;

public class a_null_binding_for_generic_builder
{
    protected static Binding binding;
    protected static BindingBuilder<string> builder;

    Establish context = () =>
    {
        binding = new Binding(typeof(string), new Strategies.Null(), new Scopes.Transient());
        builder = new BindingBuilder<string>(binding);
    };
}