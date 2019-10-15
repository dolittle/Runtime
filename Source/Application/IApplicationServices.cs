/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApplicationServices
    {
        /// <summary>
        /// Gets the <see cref="ClientServiceDefinition"/> for all exposed services
        /// </summary>
        IEnumerable<ClientServiceDefinition> Services { get; }
    }
}