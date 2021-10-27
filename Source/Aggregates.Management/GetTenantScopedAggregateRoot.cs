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
        readonly FactoryFor<IAggregates> _getAggregates;
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTenantScopedAggregateRoot"/> class.
        /// </summary>
        /// <param name="getAggregates">The <see cref="FactoryFor{T}"/> <see cref="IAggregates"/>./></param>
        /// <param name="tenants">The <see cref="ITenants"/>.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/>.</param>
        public GetTenantScopedAggregateRoot(FactoryFor<IAggregates> getAggregates, ITenants tenants, IExecutionContextManager executionContextManager)
        {
            _getAggregates = getAggregates;
            _tenants = tenants;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetFor(TenantId tenant)
            => EnsureContextIsPreserved(() =>
            {
                _executionContextManager.CurrentFor(tenant);
                return GetTenantScopedAggregatesForTenant(tenant, () => _getAggregates().GetAll());
            });

        /// <inheritdoc />
        public Task<AggregateRootWithTenantScopedAggregates> GetFor(TenantId tenant, ArtifactId aggregateRoot)
            => EnsureContextIsPreserved(async () =>
            {
                _executionContextManager.CurrentFor(tenant);
                return (await GetTenantScopedAggregatesForTenant(tenant, () => _getAggregates().GetFor(aggregateRoot)).ConfigureAwait(false)).First();
            });

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetForAllTenant()
            => EnsureContextIsPreserved(() =>
            {
                return GetTenantScopedAggregatesForAllTenants(() => _getAggregates().GetAll());
            });


        /// <inheritdoc />
        public Task<AggregateRootWithTenantScopedAggregates> GetForAllTenant(ArtifactId aggregateRoot)
            => EnsureContextIsPreserved(async () =>
            {
                return (await GetTenantScopedAggregatesForAllTenants(() => _getAggregates().GetFor(aggregateRoot)).ConfigureAwait(false)).First();
            });

        async Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetTenantScopedAggregatesForAllTenants(Func<Task<IEnumerable<Aggregate>>> getAggregates)
        {
            var result = new Dictionary<AggregateRoot, AggregateRootWithTenantScopedAggregates>();
            foreach (var tenant in _tenants.All.ToList())
            {
                _executionContextManager.CurrentFor(tenant);
                var scopedAggregateRoots = await GetTenantScopedAggregatesForTenant(tenant, getAggregates).ConfigureAwait(false);
                foreach (var (root, scopedAggregates) in scopedAggregateRoots)
                {
                    if (result.TryGetValue(root, out var aggregates))
                    {
                        foreach (var aggregate in scopedAggregates)
                        {
                            aggregates.Aggregates.Add(aggregate);
                        }
                    }
                    else
                    {
                        result.Add(root, new AggregateRootWithTenantScopedAggregates(root, scopedAggregates));
                    }
                }
            }
            return result.Values;
        }

        static async Task<IEnumerable<AggregateRootWithTenantScopedAggregates>> GetTenantScopedAggregatesForTenant(TenantId tenant, Func<Task<IEnumerable<Aggregate>>> getAggregates)
        {
            var result = new Dictionary<AggregateRoot, AggregateRootWithTenantScopedAggregates>();
            var aggregatesForTenant = await getAggregates().ConfigureAwait(false);
            foreach (var aggregate in aggregatesForTenant)
            {
                var tenantScopedAggregate = new TenantScopedAggregate(tenant, aggregate.EventSource, aggregate.Version);
                if (result.TryGetValue(aggregate.Root, out var rootWithScopedAggregates))
                {
                    rootWithScopedAggregates.Aggregates.Add(tenantScopedAggregate);
                }
                else
                {
                    result.Add(aggregate.Root, new AggregateRootWithTenantScopedAggregates(aggregate.Root, new[]
                    {
                        tenantScopedAggregate
                    }));
                }
            }
            return result.Values;
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
