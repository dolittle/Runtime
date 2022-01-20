// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IResourceConfiguration"/>.
/// </summary>
[Singleton]
public class ResourceConfiguration : IResourceConfiguration
{
    readonly IEnumerable<IAmAResourceType> _resourceTypes;
    readonly ITypeFinder _typeFinder;
    readonly ILogger _logger;
    readonly IEnumerable<IRepresentAResourceType> _resourceTypeRepresentations;
    readonly IDictionary<ResourceType, ResourceTypeImplementation> _resources = new Dictionary<ResourceType, ResourceTypeImplementation>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfiguration"/> class.
    /// </summary>
    /// <param name="resourceTypes">All <see cref="IAmAResourceType"/> implementations.</param>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> used for discovering types by the resource system.</param>
    /// <param name="container"><see cref="IContainer"/> to use for getting instances.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ResourceConfiguration(IEnumerable<IAmAResourceType> resourceTypes, ITypeFinder typeFinder, IContainer container, ILogger logger)
    {
        _resourceTypes = resourceTypes;
        _typeFinder = typeFinder;
        var resourceTypeRepresentationTypes = _typeFinder.FindMultiple<IRepresentAResourceType>();
        resourceTypeRepresentationTypes.ForEach(_ => Log.DiscoveredResourceTypeRepresentation(logger, _.AssemblyQualifiedName));

        _resourceTypeRepresentations = resourceTypeRepresentationTypes.Select(_ => container.Get(_) as IRepresentAResourceType);
        ThrowIfMultipleResourcesWithSameTypeAndImplementation(_resourceTypeRepresentations);
        ThrowIfMissingServiceBindings();
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsConfigured { get; private set; }

    /// <inheritdoc/>
    public Type GetImplementationFor(Type service)
    {
        Log.GetImplementationForService(_logger, service.AssemblyQualifiedName);
        var resourceTypesRepresentationsWithService = _resourceTypeRepresentations.Where(_ => _.Bindings.ContainsKey(service));
        
        resourceTypesRepresentationsWithService.ForEach(_ => Log.ResourceTypeWitServiceBinding(_logger, _.ImplementationName));

        Log.CurrentNumResources(_logger, _resources.Count);
        _resources.ForEach(_ => Log.ResourceTypeAndImplementation(_logger, _.Key, _.Value));

        var results = resourceTypesRepresentationsWithService.Where(_ =>
        {
            var resourceType = _.Type;
            if (!_resources.ContainsKey(resourceType)) return false;
            var resourceTypeImplementation = _.ImplementationName;
            return resourceTypeImplementation == _resources[resourceType];
        }).ToArray();
        var length = results.Length;
        return length switch
        {
            0 => throw new ImplementationForServiceNotFound(service),
            > 1 => throw new MultipleImplementationsFoundForService(service),
            _ => results[0].Bindings[service]
        };
    }

    /// <inheritdoc/>
    public void ConfigureResourceTypes(IDictionary<ResourceType, ResourceTypeImplementation> resourceTypeToImplementationMap)
    {
        Log.ResourceTypesConfigured(_logger, resourceTypeToImplementationMap);
        resourceTypeToImplementationMap.ForEach(_ =>
        {
            Log.AddingResource(_logger, _.Key, _.Value);
            _resources[_.Key] = _.Value;
        });
        IsConfigured = true;
    }

    void ThrowIfMissingServiceBindings()
    {
        foreach (var resourceType in _resourceTypes)
        {
            var services = resourceType.Services;
            var resourceTypeRepresentations = _resourceTypeRepresentations.Where(_ => _.Type == resourceType.Name);
            foreach (var resourceTypeRepresentation in resourceTypeRepresentations)
            {
                ThrowIfResourceTypeRepresentationIsMissingServiceBinding(resourceTypeRepresentation, services);
            }
                
        }
    }

    static void ThrowIfResourceTypeRepresentationIsMissingServiceBinding(IRepresentAResourceType resourceTypeRepresentation, IEnumerable<Type> services)
    {
        foreach (var service in services)
        {
            if (!resourceTypeRepresentation.Bindings.ContainsKey(service))
            {
                throw new ResourceTypeRepresentationIsMissingBindingForService(resourceTypeRepresentation.Type, resourceTypeRepresentation.ImplementationName, service);
            }
        }
    }

    static void ThrowIfMultipleResourcesWithSameTypeAndImplementation(IEnumerable<IRepresentAResourceType> resourceTypeRepresentations)
    {
        var resourcesGroupedByResourceType = resourceTypeRepresentations.GroupBy(_ => _.Type);
        resourcesGroupedByResourceType.ForEach(group =>
        {
            var numResources = group.Count();
            if (group.GroupBy(_ => _.ImplementationName).Count() != numResources) throw new FoundDuplicateResourceDefinition(group.Key);
        });
    }
}
