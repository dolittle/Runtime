// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Represents an implementation of <see cref="IHostedService"/> that resolves an underlying <see cref="IHostedService"/> from a scoped <see cref="IServiceProvider"/>.
/// </summary>
[DisableAutoRegistration]
public class ScopedHostedServiceProxy : IHostedService
{
    readonly IHostedService _hostedServiceImplementation;

    public ScopedHostedServiceProxy(
        ServiceDescriptor hostedServiceDescriptor,
        ScopedHostDefinition scopedHostDefinition,
        ICreateServiceProvidersForScopedHosts serviceProviders)
    {
        var scopedServiceProvider = serviceProviders.GetServiceProviderFor(scopedHostDefinition);
        _hostedServiceImplementation = CreateHostedServiceImplementationFrom(hostedServiceDescriptor, scopedServiceProvider);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
        => _hostedServiceImplementation.StartAsync(cancellationToken);

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
        => _hostedServiceImplementation.StopAsync(cancellationToken);

    static IHostedService CreateHostedServiceImplementationFrom(ServiceDescriptor hostedServiceDescriptor, IServiceProvider scopedServiceProvider)
    {
        if (hostedServiceDescriptor.ImplementationInstance is IHostedService instance)
        {
            return instance;
        }

        if (hostedServiceDescriptor.ImplementationType != default)
        {
            var instanceFromType = scopedServiceProvider.GetRequiredService(hostedServiceDescriptor.ImplementationType);
            if (instanceFromType is IHostedService serviceFromType)
            {
                return serviceFromType;
            }
        }

        if (hostedServiceDescriptor.ImplementationFactory != default)
        {
            var instanceFromFactory = hostedServiceDescriptor.ImplementationFactory(scopedServiceProvider);
            if (instanceFromFactory is IHostedService serviceFromFactory)
            {
                return serviceFromFactory;
            }
        }

        throw new CouldNotCreateInstanceOfHostedService(hostedServiceDescriptor);
    }
}
