// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents an implementation of <see cref="IPerformActionOnAllTenants" />.
/// </summary>
public class PerformActionOnAllTenants : IPerformActionOnAllTenants
{
    readonly ITenants _tenants;
    readonly IExecutionContextManager _executionContextManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformActionOnAllTenants"/> class.
    /// </summary>
    /// <param name="tenants">The <see cref="ITenants" />.</param>
    /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
    public PerformActionOnAllTenants(ITenants tenants, IExecutionContextManager executionContextManager)
    {
        _tenants = tenants;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public void Perform(Action<TenantId> action)
    {
        var originalExecutionContext = _executionContextManager.Current;
        try
        {
            foreach (var tenant in _tenants.All.ToArray())
            {
                _executionContextManager.CurrentFor(tenant);
                action(tenant);
            }
        }
        finally
        {
            _executionContextManager.CurrentFor(originalExecutionContext);
        }
    }

    /// <inheritdoc />
    public Try TryPerform(Func<TenantId, Try> action)
    {
        var originalExecutionContext = _executionContextManager.Current;
        try
        {
            foreach (var tenant in _tenants.All.ToArray())
            {
                _executionContextManager.CurrentFor(tenant);
                var result = action(tenant);
                if (!result.Success)
                {
                    return result;
                }
            }
        }
        finally
        {
            _executionContextManager.CurrentFor(originalExecutionContext);
        }
        return Try.Succeeded();
    }

    /// <inheritdoc/>
    public async Task PerformAsync(Func<TenantId, Task> action)
    {
        var originalExecutionContext = _executionContextManager.Current;
        try
        {
            foreach (var tenant in _tenants.All.ToArray())
            {
                _executionContextManager.CurrentFor(tenant);
                await action(tenant).ConfigureAwait(false);
            }
        }
        finally
        {
            _executionContextManager.CurrentFor(originalExecutionContext);
        }
    }

    /// <inheritdoc/>
    public async Task<Try> TryPerformAsync(Func<TenantId, Task<Try>> action)
    {
        var originalExecutionContext = _executionContextManager.Current;
        try
        {
            foreach (var tenant in _tenants.All.ToArray())
            {
                _executionContextManager.CurrentFor(tenant);
                var result = await action(tenant).ConfigureAwait(false);
                if (!result.Success)
                {
                    return result;
                }
            }
        }
        finally
        {
            _executionContextManager.CurrentFor(originalExecutionContext);
        }
        return Try.Succeeded();
    }

    /// <inheritdoc/>
    public Task PerformInParallel(Func<TenantId, Task> action)
    {
        var tenants = _tenants.All.ToArray();
        if (tenants.Length == 0)
        {
            return Task.CompletedTask;
        }

        return Task.WhenAll(tenants.Select(tenant => Task.Run(() =>
        {
            _executionContextManager.CurrentFor(tenant);
            return action(tenant);
        })));
    }
}
