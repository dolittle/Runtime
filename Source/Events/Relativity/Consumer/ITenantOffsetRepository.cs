// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Retrieves the offsets for all tenants for a particular Event Horizon.
    /// </summary>
    public interface ITenantOffsetRepository
    {
        /// <summary>
        /// Gets the Offset for each tenant.
        /// </summary>
        /// <param name="tenants">Tenants to get the offset for.</param>
        /// <param name="key">Key identifying the Event Horizon (Application and Bounded Context).</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="TenantOffset"/>.</returns>
        IEnumerable<TenantOffset> Get(IEnumerable<TenantId> tenants, EventHorizonKey key);
    }
}