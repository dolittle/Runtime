// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenants"/>.
    /// </summary>
    [Singleton]
    public class Tenants : ITenants
    {
        readonly TenantsConfiguration _tenantsConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenants"/> class.
        /// </summary>
        /// <param name="tenantsConfiguration">The <see cref="TenantsConfiguration">configuration</see> for tenants.</param>
        public Tenants(TenantsConfiguration tenantsConfiguration)
        {
            _tenantsConfiguration = tenantsConfiguration;
        }

        /// <inheritdoc/>
        public IEnumerable<TenantId> All => _tenantsConfiguration.Keys;
    }
}