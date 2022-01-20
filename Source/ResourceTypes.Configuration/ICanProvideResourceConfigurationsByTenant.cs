// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Defines a system that can provide the <see cref="ITenantResourceManager"/> with resource configurations mapped by <see cref="TenantId"/>.
/// </summary>
public interface ICanProvideResourceConfigurationsByTenant
{
    /// <summary>
    /// Gets a specific configuration based on the <see cref="TenantId"/> and the <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="configurationType">The <see cref="Type"/> of the configuration.</param>
    /// <param name="tenantId">The <see cref="TenantId"/> that the configuration belongs under.</param>
    /// <param name="resourceType">The <see cref="ResourceType"/> that the configuration belongs under.</param>
    /// <returns>An instance of the configuration object asked for.</returns>
    object ConfigurationFor(Type configurationType, TenantId tenantId, ResourceType resourceType);

    /// <summary>
    /// Gets a specific configuration based on the <see cref="TenantId"/> and the <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/> that the configuration belongs under.</param>
    /// <param name="resourceType">The <see cref="ResourceType"/> that the configuration belongs under.</param>
    /// <typeparam name="T">The type of configuration object.</typeparam>
    /// <returns>An instance of the configuration object asked for.</returns>
    T ConfigurationFor<T>(TenantId tenantId, ResourceType resourceType);
}