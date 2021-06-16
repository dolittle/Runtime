// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Represents the concept of a type of <see cref="Artifact"/>.
    /// </summary>
    public record ArtifactType(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="ArtifactType"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="ArtifactType"/>.</returns>
        public static implicit operator ArtifactType(Guid id) => new(id);
    }
}