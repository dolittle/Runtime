/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenants"/>
    /// </summary>
    [Singleton]
    public class Tenants : ITenants
    {
        readonly TenantsConfiguration _tenantsConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantsConfiguration">The <see cref="TenantsConfiguration">configuration</see> for tenants</param>
        public Tenants(TenantsConfiguration tenantsConfiguration)
        {
            _tenantsConfiguration = tenantsConfiguration;
        }

        /// <inheritdoc/>
        public IEnumerable<TenantId> All => _tenantsConfiguration.Keys;
    }   
}