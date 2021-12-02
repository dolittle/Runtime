// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationFor{T}"/>.
/// </summary>
/// <typeparam name="T">Type of configuration.</typeparam>
[SingletonPerTenant]
public class ConfigurationFor<T> : IConfigurationFor<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationFor{T}"/> class.
    /// </summary>
    /// <param name="tenantResourceManager"><see cref="ITenantResourceManager"/> for managing configuration of resources.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for resolving <see cref="ExecutionContext"/>.</param>
    public ConfigurationFor(ITenantResourceManager tenantResourceManager, IExecutionContextManager executionContextManager)
    {
        Instance = tenantResourceManager.GetConfigurationFor<T>(executionContextManager.Current.Tenant);
    }

    /// <inheritdoc/>
    public T Instance { get; }
}