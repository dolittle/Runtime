// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Types;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ITenantServiceProviders"/>.
/// </summary>
public class TenantServiceProviders : ITenantServiceProviders, IDisposable
{
    readonly ILifetimeScope _globalContainer;
    readonly ClassesByLifecycle _perTenantClasses;
    readonly ConcurrentDictionary<TenantId, AutofacServiceProvider> _providers = new();

    public TenantServiceProviders(ILifetimeScope globalContainer, ClassesByLifecycle perTenantClasses)
    {
        _globalContainer = globalContainer;
        _perTenantClasses = perTenantClasses;
    }

    /// <inheritdoc />
    public IServiceProvider ForTenant(TenantId tenant)
    {
        if (_providers.TryGetValue(tenant, out var provider))
        {
            return provider;
        }

        provider = BuildTenantServiceProvider(tenant);
        if (_providers.TryAdd(tenant, provider))
        {
            return provider;
        }
        
        provider.Dispose();
        provider = _providers[tenant];
        return provider;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var (_, provider) in _providers)
        {
            provider.Dispose();
        }
    }

    AutofacServiceProvider BuildTenantServiceProvider(TenantId tenant)
    {
        var tenantContainer = _globalContainer.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(tenant);
            builder.RegisterClassesByLifecycle(_perTenantClasses);
        });
        return new AutofacServiceProvider(tenantContainer);
    }
}
