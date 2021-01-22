// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when resources for a <see cref="TenantId"/> is not found in the resource file.
    /// </summary>
    public class MissingResourceConfigurationForTenant : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingResourceConfigurationForTenant"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/> that has missing resource configuration.</param>
        public MissingResourceConfigurationForTenant(
            TenantId tenantId)
            : base($"Tenant with id '{tenantId}' does not have a any resource configurations'")
        {
        }
    }
}