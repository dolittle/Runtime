// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can add services to a per-tenant <see cref="IServiceCollection"/>.
/// </summary>
public interface ICanAddTenantServices
{
    /// <summary>
    /// Adds services for a <see cref="TenantId"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="tenant">The tenant to add services for.</param>
    /// <param name="services">The service collection to add services into.</param>
    void AddFor(TenantId tenant, IServiceCollection services);
}
