// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvideResourceConfigurationsByTenant"/>.
    /// </summary>
    [Singleton]
    public class ResourceConfigurationsByTenantProvider : ICanProvideResourceConfigurationsByTenant
    {
        readonly ResourceConfigurationsByTenant _configuration;
        readonly IDictionary<TenantId, IDictionary<ResourceType, object>> _resourceConfigurationsByTenant = new Dictionary<TenantId, IDictionary<ResourceType, object>>();
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConfigurationsByTenantProvider"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="ResourceConfigurationsByTenant"/> configuration object.</param>
        /// <param name="serializer"><see cref="ISerializer"/>.</param>
        public ResourceConfigurationsByTenantProvider(
            ResourceConfigurationsByTenant configuration,
            ISerializer serializer)
        {
            _configuration = configuration;
            _serializer = serializer;
        }

        /// <inheritdoc/>
        public object ConfigurationFor(Type configurationType, TenantId tenantId, ResourceType resourceType)
        {
            var configurationObjects = GetConfigurationObjectsFor(tenantId);
            if (configurationObjects.ContainsKey(resourceType)) return configurationObjects[resourceType];

            var configuredObject = GetResourceConfiguration(tenantId, resourceType);
            var json = _serializer.ToJson(configuredObject);
            var configurationObject = _serializer.FromJson(configurationType, json);
            configurationObjects[resourceType] = configurationObject;

            return configurationObject;
        }

        /// <inheritdoc/>
        public T ConfigurationFor<T>(TenantId tenantId, ResourceType resourceType)
            => (T)ConfigurationFor(typeof(T), tenantId, resourceType);

        dynamic GetResourceConfiguration(TenantId tenantId, ResourceType resourceType)
        {
            ThrowIfMissingConfigurationForResourceTypeForTenant(tenantId, resourceType);
            var configurationByResourceType = _configuration[tenantId];

            return configurationByResourceType[resourceType];
        }

        IDictionary<ResourceType, object> GetConfigurationObjectsFor(TenantId tenantId)
        {
            IDictionary<ResourceType, object> configurationObjects = null;

            if (_resourceConfigurationsByTenant.ContainsKey(tenantId))
                configurationObjects = _resourceConfigurationsByTenant[tenantId];

            if (configurationObjects == null)
            {
                configurationObjects = new Dictionary<ResourceType, object>();
                _resourceConfigurationsByTenant[tenantId] = configurationObjects;
            }

            return configurationObjects;
        }

        void ThrowIfMissingResourceConfigurationForTenant(TenantId tenantId)
        {
            if (!_configuration.ContainsKey(tenantId)) throw new MissingResourceConfigurationForTenant(tenantId);
        }

        void ThrowIfMissingConfigurationForResourceTypeForTenant(TenantId tenantId, ResourceType resourceType)
        {
            if (!_configuration.TryGetValue(tenantId, out var configurationByResourceType)) throw new MissingResourceConfigurationForTenant(tenantId);
            if (!configurationByResourceType.ContainsKey(resourceType)) throw new MissingResourceConfigurationForResourceTypeForTenant(tenantId, resourceType);
        }
    }
}