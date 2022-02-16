// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Exception that gets thrown when there is no <see cref="IServiceProvider"/> configured for a tenant.
/// </summary>
public class MissingServiceProviderForTenant : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingServiceProviderForTenant"/> class.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    public MissingServiceProviderForTenant(TenantId tenant)
        : base($"Tenant {tenant} has no configured service provider.")
    {
    }
}
