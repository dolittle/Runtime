/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Defines the configuration manager for <see cref="TenantsConfiguration"/>
    /// </summary>
    public interface ITenantsConfigurationManager
    {
        /// <summary>
        /// Get the current <see cref="TenantsConfiguration"/>
        /// </summary>
        TenantsConfiguration Current { get; }
    }
    
}