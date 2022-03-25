// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents an implementation of <see cref="IPerformActionsForAllTenants"/>.
/// </summary>
public class TenantActionPerformer : IPerformActionsForAllTenants
{
    readonly ITenants _allTenants;
    readonly ITenantServiceProviders _serviceProviders;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantActionPerformer"/> class.
    /// </summary>
    /// <param name="tenants">The <see cref="ITenants"/> to use to get all configured tenants.</param>
    /// <param name="serviceProviders">The <see cref="ITenantServiceProviders"/> to use to get the <see cref="IServiceProvider"/> for each tenant.</param>
    public TenantActionPerformer(ITenants tenants, ITenantServiceProviders serviceProviders)
    {
        _allTenants = tenants;
        _serviceProviders = serviceProviders;
    }

    /// <inheritdoc />
    public void Perform(Action<TenantId, IServiceProvider> callback)
    {
        foreach (var tenant in _allTenants.All.ToArray())
        {
            callback(tenant, _serviceProviders.ForTenant(tenant));
        }
    }

    /// <inheritdoc />
    public Try TryPerform(Func<TenantId, IServiceProvider, Try> callback)
    {
        foreach (var tenant in _allTenants.All.ToArray())
        {
            var result = callback(tenant, _serviceProviders.ForTenant(tenant));
            if (!result.Success)
            {
                return result;
            }
        }

        return Try.Succeeded();
    }

    /// <inheritdoc />
    public async Task PerformAsync(Func<TenantId, IServiceProvider, Task> callback)
    {
        foreach (var tenant in _allTenants.All.ToArray())
        {
            await callback(tenant, _serviceProviders.ForTenant(tenant)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<Try> TryPerformAsync(Func<TenantId, IServiceProvider, Task<Try>> callback)
    {
        foreach (var tenant in _allTenants.All.ToArray())
        {
            var result = await callback(tenant, _serviceProviders.ForTenant(tenant)).ConfigureAwait(false);
            if (!result.Success)
            {
                return result;
            }
        }
        
        return Try.Succeeded();
    }
}
