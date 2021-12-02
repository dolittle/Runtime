// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingProviderBuilder;

public class when_binding_to_generic_type : given.a_binding_provider_builder
{
    static Binding result;
    Because of = () => result = builder.Bind<string>().Build();

    It should_hold_the_service_type_specified = () => result.Service.ShouldEqual(typeof(string));
}