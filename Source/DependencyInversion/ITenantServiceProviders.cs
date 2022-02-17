// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can provide instances of <see cref="IServiceProvider"/> per tenant.
/// </summary>
public interface ITenantServiceProviders
{
    /// <summary>
    /// Gets a <see cref="IServiceProvider"/> for the specified <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The tenant to get a service provider for.</param>
    /// <returns>A service provider with services scoped to the specified tenant.</returns>
    IServiceProvider ForTenant(TenantId tenant);
}
