/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Events.Relativity.Microservice;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting <see cref="Dolittle.Artifacts.Artifact"/> to and from protobuf representations
    /// </summary>
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Convert from <see cref="Artifacts.Artifact"/> to <see cref="Artifact"/>
        /// </summary>
        /// <param name="artifact"><see cref="Artifacts.Artifact"/> to convert from</param>
        /// <returns>Converted <see cref="Artifact"/></returns>
        public static Artifact ToProtobuf(this Artifacts.Artifact artifact)
        {
            var message = new Artifact
            {
                Id = Dolittle.Protobuf.Extensions.ToProtobuf(artifact.Id),
                Generation = artifact.Generation.Value
            };

            return message;
        }

        /// <summary>
        /// Convert from <see cref="Artifact"/> to <see cref="Dolittle.Artifacts.Artifact"/>
        /// </summary>
        /// <param name="message"><see cref="Artifact"/> to convert from</param>
        /// <returns>Converted <see cref="Artifacts.Artifact"/></returns>
        public static Artifacts.Artifact ToArtifact(this Artifact message)
        {
            return new Artifacts.Artifact(
                message.Id.To<Artifacts.ArtifactId>(),
                message.Generation
            );
        }
    }
}