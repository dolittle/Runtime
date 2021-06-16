// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Represents the concept of an artifact.
    /// </summary>
    public record Artifact(ArtifactId Id, ArtifactGeneration Generation)
    {
        /// <summary>
        /// Create a new <see cref="Artifact"/> as the first generation.
        /// </summary>
        /// <returns><see cref="Artifact">New artifact</see>.</returns>
        public static Artifact New() => new(ArtifactId.New(), ArtifactGeneration.First);
    }
}