// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can create instance of <see cref="IServiceScope"/> for tenants.
/// </summary>
public interface ITenantServiceScopeFactory
{
    /// <summary>
    /// Creates an <see cref="IServiceScope"/> for the specified <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The tenant to create a service scope for.</param>
    /// <returns>A service scope with the services scoped to the specified tenant.</returns>
    IServiceScope CreateScopeForTenant(TenantId tenant);
}
