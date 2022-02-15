// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <inheritdoc/>
[Singleton]
public class TenantResourceManager : ITenantResourceManager
{
    readonly IEnumerable<IRepresentAResourceType> _resourceDefinitions;
    readonly ICanProvideResourceConfigurationsByTenant _resourceConfigurationByTenantProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantResourceManager"/> class.
    /// </summary>
    /// <param name="resourceDefinitions"><see cref="IEnumerable{T}"/> of <see cref="IRepresentAResourceType"/>.</param>
    /// <param name="resourceConfigurationByTenantProvider"><see cref="ICanProvideResourceConfigurationsByTenant"/> for providing configuration for resources.</param>
    public TenantResourceManager(IEnumerable<IRepresentAResourceType> resourceDefinitions, ICanProvideResourceConfigurationsByTenant resourceConfigurationByTenantProvider)
    {
        _resourceDefinitions = resourceDefinitions;
        _resourceConfigurationByTenantProvider = resourceConfigurationByTenantProvider;
    }

    /// <inheritdoc/>
    public T GetConfigurationFor<T>(TenantId tenantId)
        where T : class
        => _resourceConfigurationByTenantProvider.ConfigurationFor<T>(tenantId, RetrieveResourceType<T>());

    ResourceType RetrieveResourceType<T>()
    {
        var resourceTypesMatchingType = _resourceDefinitions.Where(_ => _.ConfigurationObjectType == typeof(T)).ToArray();
        var length = resourceTypesMatchingType.Length;
        return length switch
        {
            0 => throw new NoResourceTypeMatchingConfigurationType(typeof(T)),
            > 1 => throw new ConfigurationTypeMappedToMultipleResourceTypes(typeof(T)),
            _ => resourceTypesMatchingType[0].Type
        };
    }
}
