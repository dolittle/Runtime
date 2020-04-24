// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="IPerformActionOnAllTenants" />.
    /// </summary>
    public class ActionOnAllTenantsPerformer : IPerformActionOnAllTenants
    {
        readonly ITenants _tenants;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionOnAllTenantsPerformer"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        public ActionOnAllTenantsPerformer(ITenants tenants)
        {
            _tenants = tenants;
        }

        /// <inheritdoc/>
        public void Perform(Action<TenantId> action) => _tenants.All.ToArray().ForEach(action);
    }
}