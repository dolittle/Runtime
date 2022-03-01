// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ITenantServiceProviders"/> that uses Autofac child lifetime scopes.
/// </summary>
[Singleton]
public class TenantServiceProviders : ITenantServiceProviders
{
    readonly ILifetimeScope _globalContainer;
    readonly IEnumerable<ICanAddTenantServices> _serviceAdders;
    readonly ClassesByLifecycle _perTenantClasses;
    readonly ConcurrentDictionary<TenantId, AutofacServiceProvider> _providers = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantServiceProviders"/> class.
    /// </summary>
    /// <param name="globalContainer">The <see cref="ILifetimeScope"/> root container that the tenant specific IoC containers are created from.</param>
    /// <param name="serviceAdders">The <see cref="IEnumerable{T}"/> of <see cref="ICanAddTenantServices"/>.</param>
    /// <param name="perTenantClasses">The <see cref="ClassesByLifecycle"/>.</param>
    public TenantServiceProviders(ILifetimeScope globalContainer, IEnumerable<ICanAddTenantServices> serviceAdders, ClassesByLifecycle perTenantClasses)
    {
        _globalContainer = globalContainer;
        _serviceAdders = serviceAdders;
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

    AutofacServiceProvider BuildTenantServiceProvider(TenantId tenant)
    {
        var tenantContainer = _globalContainer.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(tenant);

            var services = new ServiceCollection();

            foreach (var adder in _serviceAdders)
            {
                adder.AddFor(tenant, services);
            }
            
            builder.Populate(services);
            builder.RegisterClassesByLifecycle(_perTenantClasses);
        });
        return new AutofacServiceProvider(tenantContainer);
    }
}
