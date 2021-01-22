// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Attribute that can be added in front of <see cref="Artifact"/> to indicate its unique identifier and details.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ArtifactAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactAttribute"/> class.
        /// </summary>
        /// <param name="id">String representation of a <see cref="Guid"/> for the artifact.</param>
        /// <remarks>
        /// It will default to the <see cref="ArtifactGeneration.First">first generation</see>.
        /// </remarks>
        public ArtifactAttribute(string id)
            : this(id, ArtifactGeneration.First)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactAttribute"/> class.
        /// </summary>
        /// <param name="id"><see cref="ArtifactId"/> of the artifact.</param>
        /// <param name="generation"><see cref="ArtifactGeneration"/>.</param>
        public ArtifactAttribute(string id, uint generation)
        {
            Artifact = new Artifact(Guid.Parse(id), generation);
        }

        /// <summary>
        /// Gets the represented <see cref="Artifact"/>.
        /// </summary>
        public Artifact Artifact { get; }
    }
}