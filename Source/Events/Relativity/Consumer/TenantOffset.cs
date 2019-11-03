/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Holds the Offset for a particular tenant into the Event Horizon
    /// </summary>
    public class TenantOffset : Value<TenantOffset>
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="TenantOffset" />
        /// </summary>
        /// <param name="tenant">The tenant</param>
        /// <param name="offset">The offset</param>
        public TenantOffset(TenantId tenant, ulong offset)
        {
            Tenant = tenant;
            Offset = offset;
        }

        /// <summary>
        /// The <see cref="TenantId"/> that the offset belongs to
        /// </summary>
        /// <value></value>
        public TenantId Tenant { get; }

        /// <summary>
        /// The offset for the tenant
        /// </summary>
        /// <value></value>
        public ulong Offset { get; }
    }
}