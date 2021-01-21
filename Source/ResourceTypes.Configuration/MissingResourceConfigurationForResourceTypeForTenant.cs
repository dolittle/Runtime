// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when resources for a <see cref="TenantId"/> of a given <see cref="ResourceType"/> is not found in the resource file.
    /// </summary>
    public class MissingResourceConfigurationForResourceTypeForTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingResourceConfigurationForResourceTypeForTenant"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/> the configuration is missing for.</param>
        /// <param name="resourceType">The <see cref="ResourceType"/> that's missing.</param>
        public MissingResourceConfigurationForResourceTypeForTenant(TenantId tenantId, ResourceType resourceType)
            : base($"Missing resource configuration for resource typeof {resourceType} for tenant with Id '{tenantId}'")
        {
        }
    }
}