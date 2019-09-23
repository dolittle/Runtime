/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting event source related objects to and from protobuf representations
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Convert from <see cref="Interaction.Grpc.EventSourceVersion"/> to <see cref="EventSourceVersion"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Interaction.Grpc.EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="EventSourceVersion"/></returns>
        public static EventSourceVersion ToEventSourceVersion(this Interaction.Grpc.EventSourceVersion protobuf)
        {
            return new EventSourceVersion(protobuf.Commit, protobuf.Sequence);
        }

        /// <summary>
        /// Convert from <see cref="EventSourceVersion"/> to <see cref="Interaction.Grpc.EventSourceVersion"/>
        /// </summary>
        /// <param name="version"><see cref="EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="Interaction.Grpc.EventSourceVersion"/></returns>
        public static Interaction.Grpc.EventSourceVersion ToProtobuf(this EventSourceVersion version)
        {
            return new Interaction.Grpc.EventSourceVersion
            {
                Commit = version.Commit,
                Sequence = version.Sequence
            };
        }

        /// <summary>
        /// Convert from <see cref="Interaction.Grpc.VersionedEventSource"/> to <see cref="VersionedEventSource"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Interaction.Grpc.VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="VersionedEventSource"/></returns>
        public static VersionedEventSource ToVersionedEventSource(this Interaction.Grpc.VersionedEventSource protobuf)
        {
            return new VersionedEventSource(
                protobuf.Version.ToEventSourceVersion(),
                new EventSourceKey(protobuf.EventSource.ToConcept<EventSourceId>(),protobuf.Artifact.ToConcept<ArtifactId>()));
        }

        /// <summary>
        /// Convert from <see cref="VersionedEventSource"/> to <see cref="Interaction.Grpc.VersionedEventSource"/>
        /// </summary>
        /// <param name="versionedEventSource"><see cref="VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="Interaction.Grpc.VersionedEventSource"/></returns>
        public static Interaction.Grpc.VersionedEventSource ToProtobuf(this VersionedEventSource versionedEventSource)
        {
            var source = new Interaction.Grpc.VersionedEventSource
            {
                Version = versionedEventSource.Version.ToProtobuf(),
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;
        }
    }   
}