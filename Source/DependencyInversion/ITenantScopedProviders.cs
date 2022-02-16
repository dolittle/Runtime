// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines an system that knows about <see cref="IServiceScope"/> for specific Dolittle Tenants.
/// </summary>
public interface ITenantScopedProviders
{
    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <returns>The <see cref="IServiceProvider"/>.</returns>
    IServiceScope ScopedForTenant(TenantId tenant);
    
    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <returns>The <see cref="IServiceProvider"/>.</returns>
    IServiceProvider ForTenant(TenantId tenant);
}
