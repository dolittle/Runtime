// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents the configuration for tenants.
    /// </summary>
    [Name("tenants")]
    public class TenantsConfiguration :
        ReadOnlyDictionary<TenantId, TenantConfiguration>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsConfiguration"/> class.
        /// </summary>
        /// <param name="tenants"><see cref="IDictionary{TKey, TValue}"/> with tenants and their configuration.</param>
        public TenantsConfiguration(IDictionary<TenantId, TenantConfiguration> tenants)
            : base(tenants)
        {
        }
    }
}