// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Defines a system that perform an action for <see cref="ITenants.All" /> in an <see cref="ExecutionContext" /> with the tenant set for each Tenant.
    /// </summary>
    public interface IPerformActionOnAllTenants
    {
        /// <summary>
        /// Perform an <see cref ="Action" /> for <see cref="ITenants.All" />.
        /// </summary>
        /// <param name="action">The <see cref="Action" />.</param>
        void Perform(Action<TenantId> action);
    }
}