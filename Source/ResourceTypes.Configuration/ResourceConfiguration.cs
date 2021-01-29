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

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Represents an implementation of <see cref="IResourceConfiguration"/>.
    /// </summary>
    [Singleton]
    public class ResourceConfiguration : IResourceConfiguration
    {
        readonly ITypeFinder _typeFinder;
        readonly ILogger _logger;
        readonly IEnumerable<IRepresentAResourceType> _resourceTypeRepresentations;
        readonly IDictionary<ResourceType, ResourceTypeImplementation> _resources = new Dictionary<ResourceType, ResourceTypeImplementation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConfiguration"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> used for discovering types by the resource system.</param>
        /// <param name="container"><see cref="IContainer"/> to use for getting instances.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ResourceConfiguration(ITypeFinder typeFinder, IContainer container, ILogger logger)
        {
            logger.LogDebug("ResourceConfiguration() - ctor");

            _typeFinder = typeFinder;
            var resourceTypeRepresentationTypes = _typeFinder.FindMultiple<IRepresentAResourceType>();
            resourceTypeRepresentationTypes.ForEach(_ => logger.LogTrace("Discovered resource type representation : '{resourceTypeRepresentationType}'", _.AssemblyQualifiedName));

            _resourceTypeRepresentations = resourceTypeRepresentationTypes.Select(_ => container.Get(_) as IRepresentAResourceType);
            ThrowIfMultipleResourcesWithSameTypeAndImplementation(_resourceTypeRepresentations);
            _logger = logger;
        }

        /// <inheritdoc/>
        public bool IsConfigured { get; private set; }

        /// <inheritdoc/>
        public Type GetImplementationFor(Type service)
        {
            _logger.LogDebug("Get implementation for {service}", service.AssemblyQualifiedName);
            var resourceTypesRepresentationsWithService = _resourceTypeRepresentations.Where(_ => _.Bindings.ContainsKey(service));

            resourceTypesRepresentationsWithService.ForEach(_ => _logger.LogTrace("Resource type with service binding : {serviceBinding}", _.ImplementationName));

            _logger.LogTrace("Current resources : {numResources}", _resources.Count);
            _resources.ForEach(_ => _logger.LogTrace("Resource : {resourceType} - {resourceImplementation}", _.Key, _.Value));

            var results = resourceTypesRepresentationsWithService.Where(_ =>
            {
                var resourceType = _.Type;
                if (!_resources.ContainsKey(resourceType)) return false;
                var resourceTypeImplementation = _.ImplementationName;
                return resourceTypeImplementation == _resources[resourceType];
            }).ToArray();
            var length = results.Length;
            if (length == 0) throw new ImplementationForServiceNotFound(service);
            if (length > 1) throw new MultipleImplementationsFoundForService(service);

            return results[0].Bindings[service];
        }

        /// <inheritdoc/>
        public void ConfigureResourceTypes(IDictionary<ResourceType, ResourceTypeImplementation> resourceTypeToImplementationMap)
        {
            _logger.LogDebug("Resource Types Configured : {resourceTypeToImplementationMap}", resourceTypeToImplementationMap);
            resourceTypeToImplementationMap.ForEach(_ =>
            {
                _logger.LogTrace("Adding resource type '{resourceType}' with implementation {resourceImplementation}", _.Key, _.Value);
                _resources[_.Key] = _.Value;
            });
            IsConfigured = true;
        }

        void ThrowIfMultipleResourcesWithSameTypeAndImplementation(IEnumerable<IRepresentAResourceType> resourceTypeRepresentations)
        {
            var resourcesGroupedByResourceType = resourceTypeRepresentations.GroupBy(_ => _.Type);
            resourcesGroupedByResourceType.ForEach(group =>
            {
                var numResources = group.Count();
                if (group.GroupBy(_ => _.ImplementationName).Count() != numResources) throw new FoundDuplicateResourceDefinition(group.Key);
            });
        }
    }
}