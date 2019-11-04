/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// The representation of a client by its identifier
    /// </summary>
    public class HeadId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="HeadId"/>
        /// </summary>
        /// <param name="value"><see cref="HeadId"/> as <see cref="Guid"/></param>
        public static implicit operator HeadId(Guid value)
        {
            return new HeadId { Value = value };
        }
    }
}