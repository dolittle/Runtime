// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Defines a system for loading the resources from a configuration file containing the resources.
/// </summary>
public interface ITenantResourceManager
{
    /// <summary>
    /// Gets a specific configuration for a <see cref="TenantId"/>.
    /// </summary>
    /// <typeparam name="T">Type of configuration to get.</typeparam>
    /// <param name="tenantId"><see cref="TenantId"/> to get configuration for.</param>
    /// <returns>Configuration instance for the <see cref="TenantId"/>.</returns>
    T GetConfigurationFor<T>(TenantId tenantId)
        where T : class;
}