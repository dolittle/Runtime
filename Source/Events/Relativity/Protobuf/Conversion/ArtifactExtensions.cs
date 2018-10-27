/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="Dolittle.Artifacts.Artifact"/> to and from protobuf representations
    /// </summary>
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Convert from <see cref="Dolittle.Artifacts.Artifact"/> to <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifact"><see cref="Dolittle.Artifacts.Artifact"/> to convert from</param>
        /// <returns>Converted <see cref="Artifact"/></returns>
        public static Artifact ToProtobuf(this Dolittle.Artifacts.Artifact artifact)
        {
            var message = new Artifact();
            message.Id = artifact.Id.ToProtobuf();
            message.Generation = artifact.Generation.Value;

            return message;
        }

        /// <summary>
        /// Convert from <see cref="Artifact"/> to <see cref="Dolittle.Artifacts.Artifact"/>
        /// </summary>
        /// <param name="message"><see cref="Artifact"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Artifacts.Artifact"/></returns>
        public static Dolittle.Artifacts.Artifact ToArtifact(this Artifact message)
        {
            return new Dolittle.Artifacts.Artifact(
                message.Id.ToConcept<ArtifactId>(),
                message.Generation
            );
        }
    }
}