// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.DependencyInversion.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion.Building;

public class ServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    readonly AutofacServiceProviderFactory _factory = new();

    /// <inheritdoc />
    public ContainerBuilder CreateBuilder(IServiceCollection services)
        => _factory.CreateBuilder(services);

    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        var discoveredClasses = TypeScanner.ScanRuntimeAssemblies();
        
        containerBuilder.RegisterClassesByLifecycle(discoveredClasses.Global);

        containerBuilder.Register(
                _ => new TenantServiceProviders(
                    _.Resolve<ILifetimeScope>(),
                    discoveredClasses.PerTenant))
                .As<ITenantServiceProviders>();

        return _factory.CreateServiceProvider(containerBuilder);
    }
}
