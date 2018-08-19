/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Defines the manager for <see cref="Configuration"/>
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the current <see cref="Configuration"/>
        /// </summary>
        Configuration Current { get; }
    }

}