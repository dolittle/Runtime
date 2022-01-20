// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Discovered resource type representation : '{ResourceTypeRepresentationType}'")]
    internal static partial void DiscoveredResourceTypeRepresentation(ILogger logger, string resourceTypeRepresentationType);
    
    [LoggerMessage(0, LogLevel.Debug, "Get implementation for {Service}")]
    internal static partial void GetImplementationForService(ILogger logger, string service);
    
    [LoggerMessage(0, LogLevel.Trace, "Resource type with service binding : {ServiceBinding}")]
    internal static partial void ResourceTypeWitServiceBinding(ILogger logger, string serviceBinding);
    
    [LoggerMessage(0, LogLevel.Trace, "Current number of resources : {NumResources}")]
    internal static partial void CurrentNumResources(ILogger logger, int numResources);
    
    [LoggerMessage(0, LogLevel.Trace, "Resource : {ResourceType} - {ResourceImplementation}")]
    internal static partial void ResourceTypeAndImplementation(ILogger logger, ResourceType resourceType, ResourceTypeImplementation resourceImplementation);
    
    [LoggerMessage(0, LogLevel.Debug, "Resource Types Configured : {ResourceTypeToImplementationMap}")]
    internal static partial void ResourceTypesConfigured(ILogger logger, IDictionary<ResourceType, ResourceTypeImplementation> resourceTypeToImplementationMap);
    
    [LoggerMessage(0, LogLevel.Trace, "Adding resource type {ResourceType} with implementation {ResourceImplementation}")]
    internal static partial void AddingResource(ILogger logger, ResourceType resourceType, ResourceTypeImplementation resourceImplementation);
}
