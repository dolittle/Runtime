// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.for_BindingProviderBuilder.given;

public class a_binding_provider_builder
{
    protected static BindingProviderBuilder builder;

    Establish context = () => builder = new BindingProviderBuilder();
}