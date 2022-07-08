// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Exception that gets thrown when a specific tenant is not configured.
/// </summary>
public class TenantNotConfigured : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantNotConfigured"/> class.
    /// </summary>
    /// <param name="tenant">The tenant the event store is started for.</param>
    /// <param name="configuredTenants">The currently configured tenants.</param>
    public TenantNotConfigured(TenantId tenant, IEnumerable<TenantId> configuredTenants)
        : base($"Tenant {tenant} is not configured in the 'dolittle:runtime:tenants' configuration. These are the tenants that the Runtime knows about: {string.Join(", ", configuredTenants)}")
    {
    }
}
