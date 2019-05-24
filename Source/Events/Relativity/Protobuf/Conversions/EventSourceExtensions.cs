/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;
using Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting event source related objects to and from protobuf representations
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.EventSourceVersion"/> to <see cref="EventSourceVersion"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="EventSourceVersion"/></returns>
        public static EventSourceVersion ToEventSourceVersion(this Runtime.Grpc.Interaction.Protobuf.EventSourceVersion protobuf)
        {
            return new EventSourceVersion(protobuf.Commit, protobuf.Sequence);
        }

        /// <summary>
        /// Convert from <see cref="EventSourceVersion"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.EventSourceVersion"/>
        /// </summary>
        /// <param name="version"><see cref="EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.EventSourceVersion"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.EventSourceVersion ToProtobuf(this EventSourceVersion version)
        {
            return new Runtime.Grpc.Interaction.Protobuf.EventSourceVersion
            {
                Commit = version.Commit,
                Sequence = version.Sequence
            };
        }

        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.VersionedEventSource"/> to <see cref="VersionedEventSource"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="VersionedEventSource"/></returns>
        public static VersionedEventSource ToVersionedEventSource(this Runtime.Grpc.Interaction.Protobuf.VersionedEventSource protobuf)
        {
            return new VersionedEventSource(
                protobuf.Version.ToEventSourceVersion(),
                new EventSourceKey(protobuf.EventSource.ToConcept<EventSourceId>(),protobuf.Artifact.ToConcept<ArtifactId>()));
        }

        /// <summary>
        /// Convert from <see cref="VersionedEventSource"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.VersionedEventSource"/>
        /// </summary>
        /// <param name="versionedEventSource"><see cref="VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.VersionedEventSource"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.VersionedEventSource ToProtobuf(this Dolittle.Runtime.Events.VersionedEventSource versionedEventSource)
        {
            var source = new Runtime.Grpc.Interaction.Protobuf.VersionedEventSource
            {
                Version = versionedEventSource.Version.ToProtobuf(),
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;
        }
    }   
}