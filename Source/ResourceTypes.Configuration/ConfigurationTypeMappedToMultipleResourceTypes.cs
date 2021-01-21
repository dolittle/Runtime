// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type"/> is mapped up to multiple <see cref="ResourceType"/>.
    /// </summary>
    public class ConfigurationTypeMappedToMultipleResourceTypes : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTypeMappedToMultipleResourceTypes"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> that is mapped for multiple resource types.</param>
        public ConfigurationTypeMappedToMultipleResourceTypes(Type type)
            : base($"The type {type.FullName} is mapped up to multiple Resource Types")
        {
        }
    }
}