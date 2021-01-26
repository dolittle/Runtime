// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Represents the concept of a unique identifier for an artifact.
    /// </summary>
    public record ArtifactId(Guid Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="Guid"/> to an <see cref="ArtifactId"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> representation.</param>
        /// <returns>The converted <see cref="ArtifactId"/>.</returns>
        public static implicit operator ArtifactId(Guid id) => new(id);

        /// <summary>
        /// Create a new <see cref="ArtifactId"/>.
        /// </summary>
        /// <returns><see cref="ArtifactId">New artifact identifier</see>.</returns>
        public static ArtifactId New() => Guid.NewGuid();
    }
}