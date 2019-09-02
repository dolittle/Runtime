/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// The representation of a client by its identifier
    /// </summary>
    public class ClientId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="ClientId"/>
        /// </summary>
        /// <param name="value"><see cref="ClientId"/> as <see cref="Guid"/></param>
        public static implicit operator ClientId(Guid value)
        {
            return new ClientId { Value = value };
        }
    }
}