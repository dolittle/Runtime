// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy.for_InstancesPerTenant;

public class when_resolving_callback_binding_with_binding_context_twice_for_same_tenant : given.no_instances
{
    static object first_instance;
    static object second_instance;
    static Binding binding;

    Establish context = () =>
    {
        binding = new Binding(typeof(string), new Strategies.CallbackWithBindingContext((c) => new object()), new Scopes.SingletonPerTenant());
        tenant_key_creator.Setup(_ => _.GetKeyFor(binding, Moq.It.IsAny<Type>())).Returns("SomeKey");
    };

    Because of = () =>
    {
        first_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
        second_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
    };

    It should_return_an_instance_for_the_first_instance = () => first_instance.ShouldNotBeNull();
    It should_return_an_instance_for_the_second_instance = () => second_instance.ShouldNotBeNull();

    It should_resolve_to_same_instance = () => second_instance.ShouldEqual(first_instance);
}