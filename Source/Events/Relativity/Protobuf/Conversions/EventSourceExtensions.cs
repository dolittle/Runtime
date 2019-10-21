/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;
using Dolittle.Protobuf;
using Dolittle.Runtime.Protobuf;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting event source related objects to and from protobuf representations
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Convert from <see cref="grpc.EventSourceVersion"/> to <see cref="EventSourceVersion"/>
        /// </summary>
        /// <param name="protobuf"><see cref="grpc.EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="EventSourceVersion"/></returns>
        public static EventSourceVersion ToEventSourceVersion(this grpc.EventSourceVersion protobuf)
        {
            return new EventSourceVersion(protobuf.Commit, protobuf.Sequence);
        }

        /// <summary>
        /// Convert from <see cref="EventSourceVersion"/> to <see cref="grpc.EventSourceVersion"/>
        /// </summary>
        /// <param name="version"><see cref="EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="grpc.EventSourceVersion"/></returns>
        public static grpc.EventSourceVersion ToProtobuf(this EventSourceVersion version)
        {
            return new grpc.EventSourceVersion
            {
                Commit = version.Commit,
                Sequence = version.Sequence
            };
        }

        /// <summary>
        /// Convert from <see cref="grpc.VersionedEventSource"/> to <see cref="VersionedEventSource"/>
        /// </summary>
        /// <param name="protobuf"><see cref="grpc.VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="VersionedEventSource"/></returns>
        public static VersionedEventSource ToVersionedEventSource(this grpc.VersionedEventSource protobuf)
        {
            return new VersionedEventSource(
                protobuf.Version.ToEventSourceVersion(),
                new EventSourceKey(protobuf.EventSource.To<EventSourceId>(),protobuf.Artifact.To<ArtifactId>()));
        }

        /// <summary>
        /// Convert from <see cref="VersionedEventSource"/> to <see cref="grpc.VersionedEventSource"/>
        /// </summary>
        /// <param name="versionedEventSource"><see cref="VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="grpc.VersionedEventSource"/></returns>
        public static grpc.VersionedEventSource ToProtobuf(this VersionedEventSource versionedEventSource)
        {
            var source = new grpc.VersionedEventSource
            {
                Version = versionedEventSource.Version.ToProtobuf(),
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;
        }
    }
}