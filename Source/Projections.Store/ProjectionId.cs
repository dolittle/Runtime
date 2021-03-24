// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Represents the identification of a projection.
    /// </summary>
    public record ProjectionId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="ProjectionId"/>.
        /// </summary>
        /// <param name="projectionId">ProjectionId as <see cref="Guid"/>.</param>
        public static implicit operator ProjectionId(Guid projectionId) => new(projectionId);

        /// <summary>
        /// Creates a new instance of <see cref="ProjectionId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="ProjectionId"/>.</returns>
        public static ProjectionId New() => Guid.NewGuid();
    }
}
