/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/


using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Retrieves the offsets for all tenants for a particular Event Horizon
    /// </summary>
    public interface ITenantOffsetRepository
    {
        /// <summary>
        /// Gets the Offset for each tenant
        /// </summary>
        /// <param name="tenants">Tenants to get the offset for</param>
        /// <param name="key">Key identifying the Event Horizon (Application and Bounded Context)</param>
        /// <returns></returns>
        IEnumerable<TenantOffset> Get(IEnumerable<TenantId> tenants, EventHorizonKey key);
    }
}