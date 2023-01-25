// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Extension methods for <see cref="IPerformActionsForAllTenants"/>.
/// </summary>
public static class TenantActionPerformerExtensions
{
    /// <summary>
    /// Perform an action on the <typeparamref name="TService"/> for all tenants.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    public static void PerformOn<TService>(this IPerformActionsForAllTenants performer, Action<TService> callback) where TService: notnull
        => performer.Perform((_, services) => callback(services.GetRequiredService<TService>()));
    
    /// <summary>
    /// Try to perform an action on the <typeparamref name="TService"/> for all tenants, stopping on the first failure.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    /// <returns>The first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    public static Try TryPerformOn<TService>(this IPerformActionsForAllTenants performer, Func<TService, Try> callback) where TService: notnull
        => performer.TryPerform((_, services) => callback(services.GetRequiredService<TService>()));
    /// <summary>
    /// Perform asynchronous an action on the <typeparamref name="TService"/> for all tenants.
    /// The callback is called in sequence by waiting for each <see cref="Task"/> to complete.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    /// <returns>A <see cref="Task"/> that is resolved when the callback is called for all tenants.</returns>
    public static Task PerformAsyncOn<TService>(this IPerformActionsForAllTenants performer, Func<TService, Task> callback) where TService: notnull
        => performer.PerformAsync((_, services) => callback(services.GetRequiredService<TService>()));
    
    /// <summary>
    /// Perform asynchronous an action on the <typeparamref name="TService"/> for all tenants.
    /// The callback is called in sequence by waiting for each <see cref="Task"/> to complete.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service, tenant and service provider for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    /// <returns>A <see cref="Task"/> that is resolved when the callback is called for all tenants.</returns>
    public static Task PerformAsyncOn<TService>(this IPerformActionsForAllTenants performer, Func<TService, TenantId, IServiceProvider, Task> callback) where TService: notnull
        => performer.PerformAsync((tenant, services) => callback(services.GetRequiredService<TService>(), tenant, services));
    
    
    /// <summary>
    /// Try to perform an asynchronous action on the <typeparamref name="TService"/> for all tenants,
    /// The callback is called in sequence by waiting for each <see cref="Task{T}"/> to complete, stopping on the first failure.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    public static Task<Try> TryPerformAsyncOn<TService>(this IPerformActionsForAllTenants performer, Func<TService, Task<Try>> callback) where TService: notnull
        => performer.TryPerformAsync((_, services) => callback(services.GetRequiredService<TService>()));
    
    /// <summary>
    /// Try to perform an asynchronous action on the <typeparamref name="TService"/> for all tenants,
    /// The callback is called in sequence by waiting for each <see cref="Task{T}"/> to complete, stopping on the first failure.
    /// </summary>
    /// <param name="performer">This <see cref="IPerformActionsForAllTenants"/>.</param>
    /// <param name="callback">The callback to call with the service, tenant and service provider for each tenant.</param>
    /// <typeparam name="TService">The type of the service to get from the per-tenant service providers.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the first <see cref="Try"/> that failed, or success if all succeeded.</returns>
    public static Task<Try> TryPerformAsyncOn<TService>(this IPerformActionsForAllTenants performer, Func<TService, TenantId, IServiceProvider, Task<Try>> callback) where TService: notnull
        => performer.TryPerformAsync((tenant, services) => callback(services.GetRequiredService<TService>(), tenant, services));
}
