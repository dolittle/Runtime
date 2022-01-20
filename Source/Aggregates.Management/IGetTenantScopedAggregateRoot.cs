// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// Defines a system that can get Tenant Scoped Aggregates.
/// </summary>
public interface IGetTenantScopedAggregateRoot
{
    /// <summary>
    /// Gets all <see cref="AggregateRootWithTenantScopedInstances"/> scoped to a specific Tenant.
    /// </summary>
    /// <param name="tenant">The Tenant.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of <see cref="AggregateRootWithTenantScopedInstances"/>.</returns>
    Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetAllAggregateRootsFor(TenantId tenant);
        
    /// <summary>
    /// Gets all <see cref="AggregateRootWithTenantScopedInstances"/> for all Tenants.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of <see cref="AggregateRootWithTenantScopedInstances"/>.</returns>
    Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetAllAggregateRootsForAllTenants();
        
    /// <summary>
    /// Gets a single <see cref="AggregateRootWithTenantScopedInstances"/> scoped to a specific Tenant.
    /// </summary>
    /// <param name="tenant">The Tenant.</param>
    /// <param name="aggregateRootId">The Aggregate Root.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="AggregateRootWithTenantScopedInstances"/>.</returns>
    Task<AggregateRootWithTenantScopedInstances> GetAggregateRootFor(TenantId tenant, ArtifactId aggregateRootId);
        
    /// <summary>
    /// Gets a single <see cref="AggregateRootWithTenantScopedInstances"/> for all Tenants.
    /// </summary>
    /// <param name="aggregateRootId">The Aggregate Root.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="AggregateRootWithTenantScopedInstances"/>.</returns>
    Task<AggregateRootWithTenantScopedInstances> GetAggregateRootForAllTenants(ArtifactId aggregateRootId);
}