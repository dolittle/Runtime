// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.DependencyInversion.Tenancy;

/// <summary>
/// Exception that gets thrown when autofac fails to resolve a tenant specific service.
/// </summary>
public class FailedToResolveServiceForTenant : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToResolveServiceForTenant"/> class.
    /// </summary>
    /// <param name="tenant">The tenant id it fails to resolve service for.</param>
    /// <param name="service">The service it fails to resolve.</param>
    /// <param name="resolutionException">The inner autofac resolution exception.</param>
    public FailedToResolveServiceForTenant(TenantId tenant, Type service, Exception resolutionException)
        : base($"Failed to resolve tenant specific service {service} for tenant {tenant}", resolutionException)
    {
    }
}
