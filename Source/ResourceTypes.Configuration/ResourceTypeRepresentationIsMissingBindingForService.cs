// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when a resource type representation is missing a service binding.
    /// </summary>
    public class ResourceTypeRepresentationIsMissingBindingForService : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTypeRepresentationIsMissingBindingForService"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implementationName"></param>
        /// <param name="service"></param>
        public ResourceTypeRepresentationIsMissingBindingForService(ResourceType type, ResourceTypeImplementation implementationName, Type service)
            : base($"Resource type representation for resource type '{type.Value}' and implementation for '{implementationName.Value}' is missing service binding for {service}")
        {
        }
    }
}
