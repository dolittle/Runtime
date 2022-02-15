// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingProviderBuilder;

public class when_building_with_two_bindings : given.a_binding_provider_builder
{
    static IBindingCollection result;

    Establish context = () =>
    {
        builder.Bind(typeof(string));
        builder.Bind(typeof(object));
    };

    Because of = () => result = builder.Build();

    It should_contain_the_first_binding = () => result.ToArray()[0].Service.ShouldEqual(typeof(string));
    It should_contain_the_second_binding = () => result.ToArray()[1].Service.ShouldEqual(typeof(object));
}