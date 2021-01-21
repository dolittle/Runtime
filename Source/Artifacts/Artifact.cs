// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Represents the concept of an artifact.
    /// </summary>
    public class Artifact : Value<Artifact>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Artifact"/> class.
        /// </summary>
        /// <param name="id"><see cref="ArtifactId">Id</see> of the <see cref="Artifact"/>.</param>
        /// <param name="generation"><see cref="ArtifactGeneration">Generation</see> of the <see cref="Artifact"/>.</param>
        public Artifact(ArtifactId id, ArtifactGeneration generation)
        {
            Id = id;
            Generation = generation;
        }

        /// <summary>
        /// Gets the <see cref="Guid">unique identifier</see> of the <see cref="Artifact"/>.
        /// </summary>
        public ArtifactId Id { get; }

        /// <summary>
        /// Gets the <see cref="ArtifactGeneration">generation</see> of the <see cref="Artifact"/>.
        /// </summary>
        public ArtifactGeneration Generation { get; }

        /// <summary>
        /// Create a new <see cref="Artifact"/> as the first generation.
        /// </summary>
        /// <returns><see cref="Artifact">New artifact</see>.</returns>
        public static Artifact New()
        {
            return new Artifact(ArtifactId.New(), ArtifactGeneration.First);
        }
    }
}