// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// Represents an implementation of <see cref="IGetTenantScopedAggregateRoot"/>.
/// </summary>
[Singleton]
public class GetTenantScopedAggregateRoot : IGetTenantScopedAggregateRoot
{
    readonly FactoryFor<IAggregateRootInstances> _getAggregateRootInstances;
    readonly IAggregateRoots _aggregateRoots;
    readonly IPerformActionOnAllTenants _onAllTenants;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTenantScopedAggregateRoot"/> class.
    /// </summary>
    /// <param name="getAggregateRootInstances">The <see cref="FactoryFor{T}"/> <see cref="IAggregateRootInstances"/>./></param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
    /// <param name="onAllTenants">The performer to use to fetch aggregate root instances for all tenants.</param>
    public GetTenantScopedAggregateRoot(
        FactoryFor<IAggregateRootInstances> getAggregateRootInstances,
        IAggregateRoots aggregateRoots,
        IPerformActionOnAllTenants onAllTenants)
    {
        _getAggregateRootInstances = getAggregateRootInstances;
        _aggregateRoots = aggregateRoots;
        _onAllTenants = onAllTenants;
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
        if (!_aggregateRoots.TryGet(aggregateRootId, out var aggregateRoot))
        {
            aggregateRoot = new AggregateRoot(new AggregateRootId(aggregateRootId, ArtifactGeneration.First));
        }
            
        var instances = await GetFor(new[] {aggregateRoot}, shouldFetchForTenant).ConfigureAwait(false);
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

        await _onAllTenants.PerformAsync(async tenantId =>
        {
            if (!shouldFetchForTenant(tenantId))
            {
                return;
            }

            var forTenant = await _getAggregateRootInstances().GetFor(aggregateRoot.Identifier).ConfigureAwait(false);
            instances.AddRange(forTenant.Instances.Select(_ => new TenantScopedAggregateRootInstance(tenantId, _)));
        }).ConfigureAwait(false);

        return new AggregateRootWithTenantScopedInstances(aggregateRoot, instances);
    }
}