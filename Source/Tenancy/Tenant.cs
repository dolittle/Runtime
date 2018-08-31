/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// 
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public Tenant(TenantId id)
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public TenantId Id { get; }
        
    }
}