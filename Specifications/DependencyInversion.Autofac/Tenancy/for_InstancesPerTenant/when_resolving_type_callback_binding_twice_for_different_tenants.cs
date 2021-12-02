// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy.for_InstancesPerTenant;

public class when_resolving_type_callback_binding_twice_for_different_tenants : given.no_instances
{
    static object first_instance;
    static object second_instance;
    static Binding binding;

    static string current_tenant;

    Establish context = () =>
    {
        binding = new Binding(typeof(string), new Strategies.TypeCallback(() => typeof(string)), new Scopes.SingletonPerTenant());
        tenant_key_creator.Setup(_ => _.GetKeyFor(binding, Moq.It.IsAny<Type>())).Returns(() => current_tenant);
        type_activator.Setup(_ =>
            _.CreateInstanceFor(
                Moq.It.IsAny<IComponentContext>(),
                Moq.It.IsAny<Type>(),
                Moq.It.IsAny<Type>())).Returns(() => Guid.NewGuid().ToString());
    };

    Because of = () =>
    {
        current_tenant = "First Tenant";
        first_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
        current_tenant = "Second Tenant";
        second_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
    };

    It should_return_an_instance_for_the_first_instance = () => first_instance.ShouldNotBeNull();
    It should_return_an_instance_for_the_second_instance = () => second_instance.ShouldNotBeNull();
    It should_resolve_to_different_instance = () => second_instance.ShouldNotEqual(first_instance);
}