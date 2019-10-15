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
        /// Convert from <see cref="Dolittle.Events.Relativity.Microservice.EventSourceVersion"/> to <see cref="EventSourceVersion"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Dolittle.Events.Relativity.Microservice.EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="EventSourceVersion"/></returns>
        public static EventSourceVersion ToEventSourceVersion(this Dolittle.Events.Relativity.Microservice.EventSourceVersion protobuf)
        {
            return new EventSourceVersion(protobuf.Commit, protobuf.Sequence);
        }

        /// <summary>
        /// Convert from <see cref="EventSourceVersion"/> to <see cref="Dolittle.Events.Relativity.Microservice.EventSourceVersion"/>
        /// </summary>
        /// <param name="version"><see cref="EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Events.Relativity.Microservice.EventSourceVersion"/></returns>
        public static Dolittle.Events.Relativity.Microservice.EventSourceVersion ToProtobuf(this EventSourceVersion version)
        {
            return new Dolittle.Events.Relativity.Microservice.EventSourceVersion
            {
                Commit = version.Commit,
                Sequence = version.Sequence
            };
        }

        /// <summary>
        /// Convert from <see cref="Dolittle.Events.Relativity.Microservice.VersionedEventSource"/> to <see cref="VersionedEventSource"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Dolittle.Events.Relativity.Microservice.VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="VersionedEventSource"/></returns>
        public static VersionedEventSource ToVersionedEventSource(this Dolittle.Events.Relativity.Microservice.VersionedEventSource protobuf)
        {
            return new VersionedEventSource(
                protobuf.Version.ToEventSourceVersion(),
                new EventSourceKey(protobuf.EventSource.ToConcept<EventSourceId>(),protobuf.Artifact.ToConcept<ArtifactId>()));
        }

        /// <summary>
        /// Convert from <see cref="VersionedEventSource"/> to <see cref="Dolittle.Events.Relativity.Microservice.VersionedEventSource"/>
        /// </summary>
        /// <param name="versionedEventSource"><see cref="VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Events.Relativity.Microservice.VersionedEventSource"/></returns>
        public static Dolittle.Events.Relativity.Microservice.VersionedEventSource ToProtobuf(this VersionedEventSource versionedEventSource)
        {
            var source = new Dolittle.Events.Relativity.Microservice.VersionedEventSource
            {
                Version = versionedEventSource.Version.ToProtobuf(),
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;
        }
    }   
}