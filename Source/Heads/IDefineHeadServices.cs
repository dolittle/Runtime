/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Defines a system that defines application client services
    /// </summary>
    public interface IDefineHeadServices
    {
        /// <summary>
        /// Gets the <see cref="HeadServiceDefinition"/>
        /// </summary>
        IEnumerable<HeadServiceDefinition> Services { get; }
    }
}