// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy.for_InstancesPerTenant;

public class when_resolving_constant_binding_twice_for_same_tenant : given.no_instances
{
    static object first_instance;
    static object second_instance;
    static Binding binding;

    static string constant = "Some Constant";

    Establish context = () =>
    {
        binding = new Binding(typeof(string), new Strategies.Constant(constant), new Scopes.SingletonPerTenant());
        tenant_key_creator.Setup(_ => _.GetKeyFor(binding, Moq.It.IsAny<Type>())).Returns("SomeKey");
        type_activator.Setup(_ =>
            _.CreateInstanceFor(
                Moq.It.IsAny<IComponentContext>(),
                Moq.It.IsAny<Type>(),
                Moq.It.IsAny<Type>())).Callback(() => new object());
    };

    Because of = () =>
    {
        first_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
        second_instance = instances_per_tenant.Resolve(Mock.Of<IComponentContext>(), binding, typeof(string));
    };

    It should_resolve_first_instance_to_constant = () => first_instance.ShouldEqual(constant);
    It should_resolve_second_instance_to_constant = () => second_instance.ShouldEqual(constant);
}