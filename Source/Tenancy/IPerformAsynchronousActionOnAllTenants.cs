// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Defines a system that performs an asynchronous action for <see cref="ITenants.All" /> in an <see cref="ExecutionContext" /> with the tenant set for each Tenant.
    /// </summary>
    public interface IPerformAsynchronousActionOnAllTenants
    {
        /// <summary>
        /// Performs a <see cref="Task" /> for <see cref="ITenants.All" /> synchronously meaning that it will for each tenant
        /// perform the <see cref="Task" /> and then wait for that to finish before perforing for the next Tenant.
        /// </summary>
        /// <param name="action">The <see cref="Func{T1, TReturn}" /> that returns the <see cref="Task" /> to perform on the <see cref="TenantId" />.</param>
        /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
        Task Perform(Func<TenantId, Task> action);

        /// <summary>
        /// Performs a <see cref="Task" /> for <see cref="ITenants.All" /> in parallel meaning that it will
        /// perform the <see cref="Task" /> For all Tenants at the same time.
        /// </summary>
        /// <param name="action">The <see cref="Func{T1, TReturn}" /> that returns the <see cref="Task" /> to perform on the <see cref="TenantId" />.</param>
        /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
        Task PerformInParallel(Func<TenantId, Task> action);
    }
}