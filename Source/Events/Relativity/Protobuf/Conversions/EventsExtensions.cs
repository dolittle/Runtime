/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Execution;
using Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion;
using Dolittle.Time;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting event related objects to and from protobuf
    /// </summary>
    public static class EventsExtensions
    {
        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.EventMetadata"/> to <see cref="EventMetadata"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.EventMetadata"/> to convert from</param>
        /// <returns>Converted <see cref="EventMetadata"/></returns>
        public static EventMetadata ToEventMetadata(this Runtime.Grpc.Interaction.Protobuf.EventMetadata protobuf)
        {
            var metadata = new EventMetadata(
                protobuf.EventId.ToConcept<EventId>(),
                protobuf.Source.ToVersionedEventSource(),
                protobuf.CorrelationId.ToConcept<CorrelationId>(),
                protobuf.Artifact.ToArtifact(),
                protobuf.Occurred.ToDateTimeOffset(),
                protobuf.OriginalContext.ToOriginalContext()
            );
            return metadata;
        }

        /// <summary>
        /// Convert from <see cref="EventMetadata"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.EventMetadata"/>
        /// </summary>
        /// <param name="metadata"><see cref="EventMetadata"/> to convert from</param>
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.EventMetadata"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.EventMetadata ToProtobuf(this EventMetadata metadata)
        {
            return new Runtime.Grpc.Interaction.Protobuf.EventMetadata {
                EventId = metadata.Id.ToProtobuf(),
                Source = metadata.VersionedEventSource.ToProtobuf(),
                CorrelationId = metadata.CorrelationId.ToProtobuf(),
                Artifact = metadata.Artifact.ToProtobuf(),
                Occurred = metadata.Occurred.ToUnixTimeMilliseconds(),
                OriginalContext = metadata.OriginalContext.ToProtobuf()
            };
        }
    }
}