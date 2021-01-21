// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the id of a reverse call.
    /// </summary>
    public class ReverseCallId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert <see cref="Guid" /> to <see cref="ReverseCallId" />.
        /// </summary>
        /// <param name="id">The id.</param>
        public static implicit operator ReverseCallId(Guid id) => new ReverseCallId { Value = id };

        /// <summary>
        /// Initializes a new instance of <see cref="ReverseCallId"/>.
        /// </summary>
        /// <returns>A new <see cref="ReverseCallId"/>.</returns>
        public static ReverseCallId New() => new ReverseCallId { Value = Guid.NewGuid() };
    }
}
