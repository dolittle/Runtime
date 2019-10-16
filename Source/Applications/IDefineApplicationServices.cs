/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Defines a system that defines application client services
    /// </summary>
    public interface IDefineApplicationServices
    {
        /// <summary>
        /// Gets the <see cref="ClientServiceDefinition"/>
        /// </summary>
        IEnumerable<ClientServiceDefinition> Services { get; }
    }
}