/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents the configuration for tenants
    /// </summary>
    public class TenantsConfiguration 
    {
        /// <summary>
        /// Get the <see cref="TenantConfiguration"/> per <see cref="TenantId"/>
        /// </summary>
        public Dictionary<TenantId, TenantConfiguration> Tenants { get; set; } = new Dictionary<TenantId, TenantConfiguration>();
    }
}