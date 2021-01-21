// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Represents a configuration for the Resource System.
    /// </summary>
    public interface IResourceConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether or not the resource configuration system is configured.
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Gets the implementation for a specific service.
        /// </summary>
        /// <param name="service"><see cref="Type"/> of service to get implementation for.</param>
        /// <returns>Implementing <see cref="Type"/>.</returns>
        Type GetImplementationFor(Type service);

        /// <summary>
        /// Sets the ResourceType to ResourceTypeImplementation mapping .
        /// </summary>
        /// <param name="resourceTypeToImplementationMap"><see cref="IDictionary{TKEy,TValye}">Map</see> of <see cref="ResourceType"/> to <see cref="ResourceTypeImplementation"/>.</param>
        void ConfigureResourceTypes(IDictionary<ResourceType, ResourceTypeImplementation> resourceTypeToImplementationMap);
    }
}