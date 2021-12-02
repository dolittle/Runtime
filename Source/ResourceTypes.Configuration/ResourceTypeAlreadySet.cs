// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Exception that gets thrown when a ResourceType has already been mapped up to a ResourceTypeImplementation.
/// </summary>
public class ResourceTypeAlreadySet : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceTypeAlreadySet"/> class.
    /// </summary>
    /// <param name="resourceType"><see cref="ResourceType"/> that is already set.</param>
    /// <param name="resourceTypeImplementation">Existing mapped <see cref="ResourceTypeImplementation"/>.</param>
    public ResourceTypeAlreadySet(ResourceType resourceType, ResourceTypeImplementation resourceTypeImplementation)
        : base($"Resource type '{resourceType}' is already mapped to implementation '{resourceTypeImplementation}'")
    {
    }
}