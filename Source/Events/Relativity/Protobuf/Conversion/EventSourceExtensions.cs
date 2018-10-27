/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting event source related objects to and from protobuf representations
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Convert from <see cref="EventSourceVersion"/> to <see cref="Dolittle.Runtime.Events.EventSourceVersion"/>
        /// </summary>
        /// <param name="protobuf"><see cref="EventSourceVersion"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Runtime.Events.EventSourceVersion"/></returns>
        public static Dolittle.Runtime.Events.EventSourceVersion ToEventSourceVersion(this EventSourceVersion protobuf)
        {
            return new Dolittle.Runtime.Events.EventSourceVersion(protobuf.Commit, protobuf.Sequence);
        }

        /// <summary>
        /// Convert from <see cref="VersionedEventSource"/> to <see cref="Dolittle.Runtime.Events.VersionedEventSource"/>
        /// </summary>
        /// <param name="protobuf"><see cref="VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Runtime.Events.VersionedEventSource"/></returns>
        public static Dolittle.Runtime.Events.VersionedEventSource ToVersionedEventSource(this VersionedEventSource protobuf)
        {
            return new Dolittle.Runtime.Events.VersionedEventSource(
                protobuf.Version.ToEventSourceVersion(),
                protobuf.EventSource.ToConcept<EventSourceId>(),
                protobuf.Artifact.ToConcept<ArtifactId>());
        }

        /// <summary>
        /// Convert from <see cref="Dolittle.Runtime.Events.VersionedEventSource"/> to <see cref="VersionedEventSource"/>
        /// </summary>
        /// <param name="versionedEventSource"><see cref="Dolittle.Runtime.Events.VersionedEventSource"/> to convert from</param>
        /// <returns>Converted <see cref="VersionedEventSource"/></returns>
        public static VersionedEventSource ToProtobuf(this Dolittle.Runtime.Events.VersionedEventSource versionedEventSource)
        {
            var source = new VersionedEventSource
            {
                Version = new EventSourceVersion
                {
                    Commit = versionedEventSource.Version.Commit,
                    Sequence = versionedEventSource.Version.Sequence
                },
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;
        }
    }   
}