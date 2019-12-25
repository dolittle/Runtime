// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Holds the Offset for a particular tenant into the Event Horizon.
    /// </summary>
    public class TenantOffset : Value<TenantOffset>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantOffset"/> class.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="offset">The offset.</param>
        public TenantOffset(TenantId tenant, ulong offset)
        {
            Tenant = tenant;
            Offset = offset;
        }

        /// <summary>
        /// Gets the <see cref="TenantId"/> that the offset belongs to.
        /// </summary>
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the offset for the tenant.
        /// </summary>
        public ulong Offset { get; }
    }
}