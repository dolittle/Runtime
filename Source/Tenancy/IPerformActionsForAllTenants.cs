// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Defines a system that perform an action for all tenants configured for the Runtime.
/// </summary>
public interface IPerformActionsForAllTenants
{
    /// <summary>
    /// Perform an action for all tenants using the per-tenant <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="callback">The callback to call with each configured tenant and its service provider.</param>
    void Perform(Action<TenantId, IServiceProvider> callback);

    /// <summary>
    /// Try to perform an action for all tenants using the per-tenant <see cref="IServiceProvider"/>, stopping on the first failure.
    /// </summary>
    /// <param name="callback">The callback to call with each configured tenant and its service provider.</param>
    /// <returns>The first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    Try TryPerform(Func<TenantId, IServiceProvider, Try> callback);

    /// <summary>
    /// Perform an asynchronous action for all tenants using the per-tenant <see cref="IServiceProvider"/>.
    /// The callback is called in sequence by waiting for each <see cref="Task"/> to complete.
    /// </summary>
    /// <param name="callback">The callback to call with each configured tenant and its service provider.</param>
    /// <returns>A <see cref="Task"/> that is resolved when the callback is called for all tenants.</returns>
    Task PerformAsync(Func<TenantId, IServiceProvider, Task> callback);
    
    /// <summary>
    /// Try to perform an asynchronous action for all tenants using the per-tenant <see cref="IServiceProvider"/>.
    /// The callback is called in sequence by waiting for each <see cref="Task{T}"/> to complete, stopping on the first failure.
    /// </summary>
    /// <param name="callback">The callback to call with each configured tenant and its service provider.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    Task<Try> TryPerformAsync(Func<TenantId, IServiceProvider, Task<Try>> callback);
}
