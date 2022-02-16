// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BaselineTypeDiscovery;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.DependencyInversion.Booting;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseAutofac(this IHostBuilder builder, IEnumerable<Assembly> assemblies = default)
    {
        assemblies ??= AssemblyFinder.FindAssemblies(
            _ => {},
            _ => _.FullName!.StartsWith("Dolittle.Runtime", StringComparison.InvariantCulture)
                && !_.FullName.Contains("Contracts", StringComparison.InvariantCulture),
            false);
        var services = new AutofacServiceProviderFactory(containerBuilder => ConfigureContainer(containerBuilder, assemblies.ToArray()));
        builder.UseServiceProviderFactory(services);
        return builder;
    }

    static void ConfigureContainer(ContainerBuilder containerBuilder, Assembly[] assemblies)
    {
        containerBuilder.Register<ConfigureTenantServices>(ctx =>
        {
            return (tenantId, builder) =>
            {
                builder.RegisterAssemblyTypes(assemblies)
                    .Where(type => Attribute.IsDefined(type, typeof(SingletonPerTenantAttribute)))
                    .AsImplementedInterfaces().SingleInstance();
                containerBuilder.RegisterInstance(tenantId);
            };
        });
        containerBuilder.RegisterAssemblyModules(assemblies);
        containerBuilder
            .RegisterAssemblyTypes(assemblies)
            .Where(type =>
            {
                var attributes = type.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is IAmAScopeDecorator)
                    {
                        return false;
                    }
                }
                return true;
            })
            .AsImplementedInterfaces();
        containerBuilder
            .RegisterAssemblyTypes(assemblies)
            .Where(type => Attribute.IsDefined(type, typeof(SingletonAttribute)))
            .AsImplementedInterfaces().SingleInstance();
    }
}

