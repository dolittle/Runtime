// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// Represents an implementation of <see cref="IGetTenantScopedAggregateRoot"/>.
/// </summary>
[Singleton]
public class GetTenantScopedAggregateRoot : IGetTenantScopedAggregateRoot
{
    readonly IAggregateRoots _aggregateRoots;
    readonly IPerformActionsForAllTenants _forAllTenants;
    readonly Func<TenantId, IAggregateRootInstances> _getAggregateRootInstancesFor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantScopedAggregateRoot"/> class.
    /// </summary>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
    /// <param name="forAllTenants">The performer to use to fetch aggregate root instances for all tenants.</param>
    /// <param name="getAggregateRootInstancesFor">The factory to use to get <see cref="IAggregateRootInstances"/> for each tenant./></param>
    public GetTenantScopedAggregateRoot(
        IAggregateRoots aggregateRoots,
        IPerformActionsForAllTenants forAllTenants,
        Func<TenantId, IAggregateRootInstances> getAggregateRootInstancesFor) 
        // TODO: Instead of this "perform for all tenants" + "func<TenantId, something>" pattern, we should really make a thing to to this with types
    {
        _getAggregateRootInstancesFor = getAggregateRootInstancesFor;
        _aggregateRoots = aggregateRoots;
        _forAllTenants = forAllTenants;
    }

    /// <inheritdoc />
    public Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetAllAggregateRootsFor(TenantId tenant)
        => GetFor(_aggregateRoots.All, _ => _ == tenant);

    /// <inheritdoc />
    public Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetAllAggregateRootsForAllTenants()
        => GetFor(_aggregateRoots.All, _ => true);

    /// <inheritdoc />
    public Task<AggregateRootWithTenantScopedInstances> GetAggregateRootFor(TenantId tenant, ArtifactId aggregateRootId)
        => GetByArtifactIdFor(aggregateRootId, _ => _ == tenant);

    /// <inheritdoc />
    public Task<AggregateRootWithTenantScopedInstances> GetAggregateRootForAllTenants(ArtifactId aggregateRootId)
        => GetByArtifactIdFor(aggregateRootId, _ => true);

    async Task<AggregateRootWithTenantScopedInstances> GetByArtifactIdFor(ArtifactId aggregateRootId, Func<TenantId, bool> shouldFetchForTenant)
    {
        if (!_aggregateRoots.TryGetFor(aggregateRootId, out var aggregateRoot))
        {
            aggregateRoot = new AggregateRoot(new AggregateRootId(aggregateRootId, ArtifactGeneration.First));
        }
            
        var instances = await GetFor(new[] {aggregateRoot!}, shouldFetchForTenant).ConfigureAwait(false);
        return instances.First();
    }

    async Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetFor(IEnumerable<AggregateRoot> aggregateRoots, Func<TenantId, bool> shouldFetchForTenant)
    {
        var result = new List<AggregateRootWithTenantScopedInstances>();

        foreach (var aggregateRoot in aggregateRoots)
        {
            var instancesForTenants =
                await GetForTenants(shouldFetchForTenant, aggregateRoot).ConfigureAwait(false);
            result.Add(instancesForTenants);
        }

        return result;
    }

    async Task<AggregateRootWithTenantScopedInstances> GetForTenants(Func<TenantId, bool> shouldFetchForTenant, AggregateRoot aggregateRoot)
    {
        var instances = new List<TenantScopedAggregateRootInstance>();

        await _forAllTenants.PerformAsync(async (tenant, _) =>
        {
            if (!shouldFetchForTenant(tenant))
            {
                return;
            }

            var forTenant = await _getAggregateRootInstancesFor(tenant).GetFor(aggregateRoot.Identifier).ConfigureAwait(false);
            instances.AddRange(forTenant.Instances.Select(__ => new TenantScopedAggregateRootInstance(tenant, __)));
        }).ConfigureAwait(false);

        return new AggregateRootWithTenantScopedInstances(aggregateRoot, instances);
    }
}
