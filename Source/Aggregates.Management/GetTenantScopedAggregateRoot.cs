// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="IGetTenantScopedAggregateRoot"/>.
    /// </summary>
    [Singleton]
    public class GetTenantScopedAggregateRoot : IGetTenantScopedAggregateRoot
    {
        readonly FactoryFor<IAggregateRootInstances> _getAggregateRootInstances;
        readonly IAggregateRoots _aggregateRoots;
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTenantScopedAggregateRoot"/> class.
        /// </summary>
        /// <param name="getAggregateRootInstances">The <see cref="FactoryFor{T}"/> <see cref="IAggregateRootInstances"/>./></param>
        /// <param name="aggregateRoots">The <see cref="IAggregateRoots"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/>.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/>.</param>
        public GetTenantScopedAggregateRoot(
            FactoryFor<IAggregateRootInstances> getAggregateRootInstances,
            IAggregateRoots aggregateRoots,
            ITenants tenants,
            IExecutionContextManager executionContextManager)
        {
            _getAggregateRootInstances = getAggregateRootInstances;
            _aggregateRoots = aggregateRoots;
            _tenants = tenants;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetFor(TenantId tenant)
            => EnsureContextIsPreserved(() =>
            {
                _executionContextManager.CurrentFor(tenant);
                return GetTenantScopedAggregateRootsForTenant(tenant, () => _getAggregateRootInstances().GetAll());
            });

        /// <inheritdoc />
        public async Task<AggregateRootWithTenantScopedInstances> GetFor(TenantId tenant, ArtifactId aggregateRootId)
        {
            if (!_aggregateRoots.TryGet(aggregateRootId, out var aggregateRoot))
            {
                throw new AggregateRootIsNotRegistered(aggregateRootId);
            }
            return await EnsureContextIsPreserved(async () =>
            {
                _executionContextManager.CurrentFor(tenant);
                var tenantScopedRoots = await GetTenantScopedAggregateRootsForTenant(
                    tenant,
                    async () =>
                    {
                        var instances = await _getAggregateRootInstances().GetFor(aggregateRoot).ConfigureAwait(false);
                        return new[]
                        {
                            new AggregateRootWithInstances(aggregateRoot, instances)
                        };
                    }
                    ).ConfigureAwait(false);
                return tenantScopedRoots.FirstOrDefault();
            }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetForAllTenant()
            => EnsureContextIsPreserved(() =>
            {
                return GetTenantScopedAggregateRootsForAllTenants(() => _getAggregateRootInstances().GetAll());
            });


        /// <inheritdoc />
        public async Task<AggregateRootWithTenantScopedInstances> GetForAllTenant(ArtifactId aggregateRootId)
        {
            if (!_aggregateRoots.TryGet(aggregateRootId, out var aggregateRoot))
            {
                throw new AggregateRootIsNotRegistered(aggregateRootId);
            }
            return await EnsureContextIsPreserved(async () =>
            {
                var tenantScopedRoots = await GetTenantScopedAggregateRootsForAllTenants(
                    async () =>
                    {
                        var instances = await _getAggregateRootInstances().GetFor(aggregateRoot).ConfigureAwait(false);
                        return new[]
                        {
                            new AggregateRootWithInstances(aggregateRoot, instances)
                        };
                    }
                ).ConfigureAwait(false);
                return tenantScopedRoots.FirstOrDefault();
            }).ConfigureAwait(false);
        }

        async Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetTenantScopedAggregateRootsForAllTenants(Func<Task<IEnumerable<AggregateRootWithInstances>>> getRootWithInstances)
        {
            var result = new Dictionary<AggregateRoot, List<TenantScopedAggregateRootInstance>>();
            foreach (var tenant in _tenants.All.ToList())
            {
                _executionContextManager.CurrentFor(tenant);
                var rootsWithInstancesForTenant = await GetTenantScopedAggregateRootsForTenant(tenant, getRootWithInstances).ConfigureAwait(false);
                foreach (var (root, instances) in rootsWithInstancesForTenant)
                {
                    if (result.TryGetValue(root, out var tenantScopedInstances))
                    {
                        tenantScopedInstances.AddRange(instances);
                    }
                    else
                    {
                        result.Add(root, instances.ToList());
                    }
                }
            }
            return result.Select(_ => new AggregateRootWithTenantScopedInstances(_.Key, _.Value));
        }

        static async Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetTenantScopedAggregateRootsForTenant(TenantId tenant, Func<Task<IEnumerable<AggregateRootWithInstances>>> getRootWithInstances)
        {
            var result = new Dictionary<AggregateRoot, List<TenantScopedAggregateRootInstance>>();
            var rootsWithInstancesForTenant = await getRootWithInstances().ConfigureAwait(false);
            foreach (var (root, instances) in rootsWithInstancesForTenant)
            {
                var newTenantScopedInstances = instances.Select(_ => new TenantScopedAggregateRootInstance(tenant, _));
                if (result.TryGetValue(root, out var tenantScopedInstances))
                {
                    tenantScopedInstances.AddRange(newTenantScopedInstances);
                }
                else
                {
                    result.Add(root, newTenantScopedInstances.ToList());
                }
            }
            return result.Select(_ => new AggregateRootWithTenantScopedInstances(_.Key, _.Value));
        }

        async Task<TResult> EnsureContextIsPreserved<TResult>(Func<Task<TResult>> action)
        {
            var oldContext = _executionContextManager.Current;
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                _executionContextManager.CurrentFor(oldContext);
            }
        }
    }
}