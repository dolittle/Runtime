// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the id of a head.
    /// </summary>
    public record HeadId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly convert <see cref="Guid" /> to <see cref="HeadId" />.
        /// </summary>
        /// <param name="id">The id.</param>
        public static implicit operator HeadId(Guid id) => new(id);
    }
}
