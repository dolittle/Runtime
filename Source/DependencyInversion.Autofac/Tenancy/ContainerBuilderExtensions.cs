// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Autofac.Core;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy;

/// <summary>
/// Extensions for <see cref="ContainerBuilder"/> related to <see cref="BindingsPerTenantsRegistrationSource"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Add <see cref="BindingsPerTenantsRegistrationSource"/> as a <see cref="IRegistrationSource"/>.
    /// </summary>
    /// <param name="containerBuilder">The <see cref="ContainerBuilder"/>.</param>
    public static void AddBindingsPerTenantRegistrationSource(this ContainerBuilder containerBuilder)
    {
        var tenantKeyCreator = new TenantKeyCreator(containerBuilder);
        var typeActivator = new TypeActivator(containerBuilder);
        var instancesPerTenant = new InstancesPerTenant(tenantKeyCreator, typeActivator);
        containerBuilder.RegisterSource(new BindingsPerTenantsRegistrationSource(instancesPerTenant));
    }
}