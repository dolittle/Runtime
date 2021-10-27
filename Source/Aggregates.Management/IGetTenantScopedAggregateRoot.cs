// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Defines a system that can get Tenant Scoped Aggregates.
    /// </summary>
    public interface IGetTenantScopedAggregateRoot
    {
        /// <summary>
        /// Gets all <see cref="AggregateRootWithTenantScopedAggregates"/> scoped to a specific Tenant.
        /// </summary>
        /// <param name="tenant">The Tenant.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of <see cref="AggregateRootWithTenantScopedAggregates"/>.</returns>
        Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetFor(TenantId tenant);
        
        /// <summary>
        /// Gets all <see cref="AggregateRootWithTenantScopedAggregates"/> for all Tenants.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of <see cref="AggregateRootWithTenantScopedAggregates"/>.</returns>
        Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetForAllTenant();
        
        /// <summary>
        /// Gets a single <see cref="AggregateRootWithTenantScopedAggregates"/> scoped to a specific Tenant.
        /// </summary>
        /// <param name="tenant">The Tenant.</param>
        /// <param name="aggregateRoot">The Aggregate Root.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="AggregateRootWithTenantScopedAggregates"/>.</returns>
        Task<AggregateRootWithTenantScopedAggregates> GetFor(TenantId tenant, ArtifactId aggregateRoot);
        
        /// <summary>
        /// Gets a single <see cref="AggregateRootWithTenantScopedAggregates"/> for all Tenants.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="AggregateRootWithTenantScopedAggregates"/>.</returns>
        Task<AggregateRootWithTenantScopedAggregates> GetForAllTenant(ArtifactId aggregateRoot);
    }
}
