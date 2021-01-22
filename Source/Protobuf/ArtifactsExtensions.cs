// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using ArtifactContract = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents conversion extensions for the common execution types.
    /// </summary>
    public static class ArtifactsExtensions
    {
        /// <summary>
        /// Convert a <see cref="Artifact"/> to <see cref="ArtifactContract"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to convert from.</param>
        /// <returns>Converted <see cref="ArtifactContract"/>.</returns>
        public static ArtifactContract ToProtobuf(this Artifact artifact) =>
            new ArtifactContract { Id = artifact.Id.ToProtobuf(), Generation = artifact.Generation.Value };

        /// <summary>
        /// Convert a <see cref="ArtifactContract"/> to <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="ArtifactContract"/> to convert from.</param>
        /// <returns>Converted <see cref="Artifact"/>.</returns>
        public static Artifact ToArtifact(this ArtifactContract artifact) =>
            new Artifact(artifact.Id.To<ArtifactId>(), artifact.Generation);
    }
}