// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration.ConfigurationObjects;
using Dolittle.Runtime.DependencyInversion.Booting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ITenantScopedProviders" />.
/// </summary>
[Singleton]
public class TenantScopedProviders : ITenantScopedProviders
{
    readonly ILifetimeScope _rootContainer;
    readonly Dictionary<TenantId, ILifetimeScope> _serviceProviders = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantScopedProviders"/> class.
    /// </summary>
    /// <param name="rootContainer"></param>
    /// <param name="tenants"></param>
    /// <param name="configureTenantServices"></param>
    public TenantScopedProviders(ILifetimeScope rootContainer, IOptions<TenantsConfiguration> tenants, ConfigureTenantServices configureTenantServices)
    {
        _rootContainer = rootContainer;
        foreach (var (tenantId, _) in tenants.Value)
        {
            _serviceProviders[tenantId] = rootContainer.BeginLifetimeScope(containerBuilder =>
            {
                configureTenantServices(tenantId, containerBuilder);
            });
        }
    }

    /// <inheritdoc />
    public IServiceScope ScopedForTenant(TenantId tenant)
        => GetLifetimeScope(tenant).Resolve<IServiceScopeFactory>().CreateScope();

    /// <inheritdoc />
    public IServiceProvider ForTenant(TenantId tenant)
        => GetLifetimeScope(tenant).Resolve<IServiceProvider>();

    ILifetimeScope GetLifetimeScope(TenantId tenant)
    {
        if (!_serviceProviders.TryGetValue(tenant, out var provider))
        {
            throw new MissingServiceProviderForTenant(tenant);
        }
        return provider;
    }
}
