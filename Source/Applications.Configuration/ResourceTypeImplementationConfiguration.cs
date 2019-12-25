// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ResourceTypes;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents a configuration that describes which <see cref="ResourceTypeImplementation"/> that's configured for a <see cref="ResourceType"/>. Used in <see cref="BoundedContextConfiguration"/> for serialization .
    /// </summary>
    public class ResourceTypeImplementationConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="ResourceTypeImplementation"/> for a <see cref="ResourceType"/> in the Production environment.
        /// </summary>
        public ResourceTypeImplementation Production { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ResourceTypeImplementation"/> for a <see cref="ResourceType"/> in the Development environment.
        /// </summary>
        public ResourceTypeImplementation Development { get; set; }
    }
}