// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ITenantServiceScopeFactory"/>.
/// </summary>
[Singleton]
public class TenantServiceScopeFactory : ITenantServiceScopeFactory
{
    readonly ITenantServiceProviders _providers;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantServiceScopeFactory"/> class.
    /// </summary>
    /// <param name="providers">The tenant service providers to use to get the service provider for a tenant.</param>
    public TenantServiceScopeFactory(ITenantServiceProviders providers)
    {
        _providers = providers;
    }

    /// <inheritdoc />
    public IServiceScope CreateScopeForTenant(TenantId tenant)
        => _providers
            .ForTenant(tenant)
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
}
