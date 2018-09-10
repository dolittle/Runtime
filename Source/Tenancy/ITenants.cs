/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Defines a system for working with <see cref="TenantId">tenants</see>
    /// </summary>
    public interface ITenants 
    {
        /// <summary>
        /// Gets all available tenants represented by their <see cref="TenantId"/>
        /// </summary>
        IEnumerable<TenantId>   All { get; }
    }
}