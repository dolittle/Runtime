/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Configuration;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents the configuration for tenants
    /// </summary>
    [Name("tenants")]
    public class TenantsConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TenantsConfiguration"/>
        /// </summary>
        /// <param name="tenants"><see cref="IDictionary{TKey, TValue}"/> with tenants and their configuration</param>
        public TenantsConfiguration(IDictionary<TenantId, TenantConfiguration> tenants)
        {
            Tenants = tenants;
        }

        /// <summary>
        /// Get the <see cref="TenantConfiguration"/> per <see cref="TenantId"/>
        /// </summary>
        public IDictionary<TenantId, TenantConfiguration> Tenants { get; }
    }
}