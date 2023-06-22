// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Configuration.DependencyInversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Represents an implementation of <see cref="IHostBuilder"/> that allows creating a new child DI container that isolates the added services from the parent host.
/// The instances of <see cref="IHostedService"/> in the scoped host will be started by the parent host, but in the context of the child DI container.
/// </summary>
public class ScopedHostBuilder : IHostBuilder
{
    readonly IHostBuilder _parentBuilder;
    readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedHostBuilder"/> class.
    /// </summary>
    /// <param name="parentBuilder">The parent <see cref="IHostBuilder"/> to base this scoped host builder on.</param>
    public ScopedHostBuilder(IHostBuilder parentBuilder)
    {
        _parentBuilder = parentBuilder;
        Properties = _parentBuilder.Properties;
        _parentBuilder.ConfigureServices(RegisterHostedServiceProxies);
    }

    /// <inheritdoc />
    public IDictionary<object, object> Properties { get; }

    /// <inheritdoc />
    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _parentBuilder.ConfigureHostConfiguration(configureDelegate);
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _parentBuilder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _configureServicesActions.Add(configureDelegate);
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        where TContainerBuilder: notnull
        => throw new OperationNotSupportedOnScopedHostBuilder(nameof(UseServiceProviderFactory));

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        where TContainerBuilder: notnull
        => throw new OperationNotSupportedOnScopedHostBuilder(nameof(UseServiceProviderFactory));

    /// <inheritdoc />
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        => throw new OperationNotSupportedOnScopedHostBuilder(nameof(ConfigureContainer));

    /// <inheritdoc />
    public IHost Build()
        => throw new OperationNotSupportedOnScopedHostBuilder(nameof(Build));

    void RegisterHostedServiceProxies(HostBuilderContext context, IServiceCollection parentServices)
    {
        var scopedServices = CreateScopedHostServiceCollection(context);
        
        // TODO: Can we do this in a nicer way? This code forces our IOptionsFactory into the host builder
        new Services().AddTo(scopedServices);

        var scopedHost = new ScopedHostDefinition(scopedServices);

        var hostedServicesToProxy = scopedServices.Where(_ => typeof(IHostedService).IsAssignableFrom(_.ServiceType));
        foreach (var descriptor in hostedServicesToProxy)
        {
            parentServices.AddSingleton<IHostedService>(parentServiceProvider => new ScopedHostedServiceProxy(
                descriptor,
                scopedHost,
                parentServiceProvider.GetRequiredService<ICreateServiceProvidersForScopedHosts>()));
        }
    }

    IServiceCollection CreateScopedHostServiceCollection(HostBuilderContext context)
    {
        var services = new ServiceCollection();
        
        foreach (var action in _configureServicesActions)
        {
            action(context, services);
        }

        return EnsureHostedServicesAreRegisteredAsConcreteTypes(services);
    }

    static IServiceCollection EnsureHostedServicesAreRegisteredAsConcreteTypes(IServiceCollection originalServices)
    {
        var services = new ServiceCollection();

        foreach (var descriptor in originalServices)
        {
            if (!IsHostedServiceRegisteredAsType(descriptor))
            {
                services.Add(descriptor);
                continue;
            }

            var hostedServiceRegisteredAsSelf = ServiceDescriptor.Singleton(descriptor.ImplementationType!, descriptor.ImplementationType!);
            services.Add(hostedServiceRegisteredAsSelf);
        }

        return services;
    }

    static bool IsHostedServiceRegisteredAsType(ServiceDescriptor descriptor)
        => typeof(IHostedService).IsAssignableFrom(descriptor.ServiceType) && descriptor.ImplementationType != default;
}
