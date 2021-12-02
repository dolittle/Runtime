// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy.for_InstancesPerTenant;

public class when_resolving_type_binding : given.no_instances
{
    static Binding binding;
    static IComponentContext component_context;

    static Type service_type;
    static Type binding_type;
    static Type type;

    Establish context = () =>
    {
        component_context = Mock.Of<IComponentContext>();
        service_type = typeof(double);
        binding_type = typeof(int);
        type = typeof(string);

        binding = new Binding(service_type, new Strategies.Type(binding_type), new Scopes.SingletonPerTenant());
        tenant_key_creator.Setup(_ => _.GetKeyFor(binding, type)).Returns("SomeKey");
    };

    Because of = () => instances_per_tenant.Resolve(component_context, binding, type);

    It should_ask_activator_to_create_instance = () => type_activator.Verify(_ => _.CreateInstanceFor(component_context, service_type, binding_type), Moq.Times.Once());
}