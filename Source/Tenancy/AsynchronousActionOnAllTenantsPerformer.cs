// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="IPerformAsynchronousActionOnAllTenants" />.
    /// </summary>
    public class AsynchronousActionOnAllTenantsPerformer : IPerformAsynchronousActionOnAllTenants
    {
        readonly ITenants _tenants;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousActionOnAllTenantsPerformer"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        public AsynchronousActionOnAllTenantsPerformer(ITenants tenants)
        {
            _tenants = tenants;
        }

        /// <inheritdoc/>
        public async Task Perform(Func<TenantId, Task> action)
        {
            var tenants = _tenants.All.ToArray();
            foreach (var tenant in tenants)
            {
                await action(tenant).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task PerformInParallel(Func<TenantId, Task> action)
        {
            var tenants = _tenants.All.ToArray();
            var tasks = tenants.Length == 0 ? new[] { Task.CompletedTask } : tenants.Select(action);
            return Task.WhenAll(tasks);
        }
    }
}