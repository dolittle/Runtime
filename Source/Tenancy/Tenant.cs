/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents a tenant
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Tenant"/>
        /// </summary>
        /// <param name="id"></param>
        public Tenant(TenantId id)
        {
            Id = id;
        }
        
        /// <summary>
        /// Gets the <see cref="TenantId"/>
        /// </summary>
        public TenantId Id { get; }
    }
}