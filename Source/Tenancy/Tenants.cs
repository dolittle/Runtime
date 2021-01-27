using System.Linq;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;

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
        public Tenants(TenantsConfiguration tenantsConfiguration) => _tenantsConfiguration = tenantsConfiguration;

        /// <inheritdoc/>
        public ObservableCollection<TenantId> All => new(_tenantsConfiguration.Keys.Select(tenant => new TenantId(tenant)));
    }
}