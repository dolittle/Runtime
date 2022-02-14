// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Defines a system that perform an action for <see cref="ITenants.All" /> in an <see cref="ExecutionContext" /> with the tenant set for each Tenant.
/// </summary>
public interface IPerformActionOnAllTenants
{
    /// <summary>
    /// Perform an action on all tenants.
    /// </summary>
    /// <param name="action">The <see cref="Action{TenantId}">action</see> to perform.</param>
    void Perform(Action<TenantId> action);

    /// <summary>
    /// Try to perform an action on all tenants, stopping on the first failure.
    /// </summary>
    /// <param name="action">The <see cref="Func{TenantId, Try}">action</see> to perform.</param>
    /// <returns>The first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    Try TryPerform(Func<TenantId, Try> action);

    /// <summary>
    /// Perform an asynchronous action on all tenants in sequence by waiting for the <see cref="Task"/> for each tenant to complete.
    /// </summary>
    /// <param name="action">The <see cref="Func{TenantId, Task}">action</see> to perform.</param>
    /// <returns>A <see cref="Task"/> that is resolved when the action is performed on all tenants.</returns>
    Task PerformAsync(Func<TenantId, Task> action);
    
    /// <summary>
    /// Try to perform an asynchronous action on all tenants in sequence by waiting for the <see cref="Task{Try}"/> for each tenant to complete, stopping on the first failure.
    /// </summary>
    /// <param name="action">The <see cref="Func{TenantId, Task}">action</see> to perform.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    Task<Try> TryPerformAsync(Func<TenantId, Task<Try>> action);

    /// <summary>
    /// Perform an asynchronous action on all tenants in parallel.
    /// </summary>
    /// <param name="action">The <see cref="Func{TenantId, Task}">action</see> to perform.</param>
    /// <returns>A <see cref="Task"/> that is resolved when the action is performed on all tenants.</returns>
    Task PerformInParallel(Func<TenantId, Task> action);
}
