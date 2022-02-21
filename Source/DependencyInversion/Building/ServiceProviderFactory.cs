// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.DependencyInversion.Attributes;
using Dolittle.Runtime.DependencyInversion.Logging;
using Dolittle.Runtime.DependencyInversion.Tenancy;
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
        var discoveredClasses = TypeScanner.GetAllClassesInRuntimeAssemblies()
            .IgnoreClassesWithAttribute<DisableAutoRegistrationAttribute>()
            .FilterClassesImplementing(typeof(ICanAddServices), out var classesThatCanAddServices)
            .FilterClassesImplementing(typeof(ICanAddTenantServices), out var classesThatCanAddTenantServices)
            .FilterClassesImplementing(typeof(ICanAddServicesForTypesWith<>), out var classesThatCanAddServicesForAttribute)
            .FilterClassesImplementing(typeof(ICanAddTenantServicesForTypesWith<>), out var classesThatCanAddTenantServicesForAttribute)
            .ToList();

        var instancesThatCanAddServices = CreateInstanceOfWithDefaultConstructor<ICanAddServices>(classesThatCanAddServices);
        var instancesThatCanAddServicesForAttribute = classesThatCanAddServicesForAttribute.Select(_ => ServicesForTypesWith.CreateBuilderFor(_, discoveredClasses));

        var instancesThatCanAddTenantServices = CreateInstanceOfWithDefaultConstructor<ICanAddTenantServices>(classesThatCanAddTenantServices);
        var instancesThatCanAddTenantServicesForAttribute = classesThatCanAddTenantServicesForAttribute.Select(_ => TenantServicesForTypesWith.CreateBuilderFor(_, discoveredClasses));

        var serviceAdders = instancesThatCanAddServices.Concat(instancesThatCanAddServicesForAttribute).ToList();
        var tenantServiceAdders = instancesThatCanAddTenantServices.Concat(instancesThatCanAddTenantServicesForAttribute).ToList();
        
        var services = new ServiceCollection();
        foreach (var builder in serviceAdders)
        {
            builder.AddTo(services);
        }
        containerBuilder.Populate(services);

        var groupedClasses = TypeScanner.GroupClassesByScopeAndLifecycle(discoveredClasses);
        containerBuilder.RegisterClassesByLifecycle(groupedClasses.Global);

        containerBuilder.Register(
                _ => new TenantServiceProviders(
                    _.Resolve<ILifetimeScope>(),
                    tenantServiceAdders,
                    groupedClasses.PerTenant))
                .As<ITenantServiceProviders>()
                .SingleInstance();

        containerBuilder.AddLogging();
        containerBuilder.AddTenantFactories();

        return _factory.CreateServiceProvider(containerBuilder);
    }

    static IEnumerable<T> CreateInstanceOfWithDefaultConstructor<T>(IEnumerable<Type> classes)
        where T : class
        => classes.Select(type =>
        {
            try
            {
                if (Activator.CreateInstance(type) is not T instance)
                {
                    throw new CouldNotCreateInstanceOfType(type);
                }

                return instance;
            }
            catch (Exception exception) when (exception is not CouldNotCreateInstanceOfType)
            {
                throw new CouldNotCreateInstanceOfType(type, exception);
            }
        });
}
