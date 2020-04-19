// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using Dolittle.Artifacts;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Google.Protobuf.WellKnownTypes;
using grpcArtifacts = contracts::Dolittle.Artifacts.Contracts;
using grpcEvents = contracts::Dolittle.Runtime.Events.Contracts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for working with conversions between <see cref="CommittedEvent"/> and <see cref="grpcEvents.CommittedEvent"/>.
    /// </summary>
    public static class CommittedEventExtensions
    {
        /// <summary>
        /// Convert to a protobuf message representation of <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="CommittedEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="grpcEvents.CommittedEvent"/>.</returns>
        public static grpcEvents.CommittedEvent ToProtobuf(this CommittedEvent @event)
        {
            return new grpcEvents.CommittedEvent
            {
                EventLogSequenceNumber = @event.EventLogSequenceNumber,
                Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
                EventSourceId = @event.EventSource.ToProtobuf(),
                ExecutionContext = @event.ExecutionContext.ToProtobuf(),
                Type = new grpcArtifacts.Artifact
                {
                    Id = @event.Type.Id.ToProtobuf(),
                    Generation = @event.Type.Generation
                },
                Content = @event.Content,
                Public = @event.Public
            };
        }

        /// <summary>
        /// Convert to from <see cref="grpcEvents.CommittedEvent"/> to <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="grpcEvents.CommittedEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="CommittedEvent"/>.</returns>
        public static CommittedEvent ToCommittedEvent(this grpcEvents.CommittedEvent @event)
        {
            return new CommittedEvent(
                @event.EventLogSequenceNumber,
                @event.Occurred.ToDateTimeOffset(),
                @event.EventSourceId.To<EventSourceId>(),
                @event.ExecutionContext.ToExecutionContext(),
                new Artifact(@event.Type.Id.To<ArtifactId>(), @event.Type.Generation),
                @event.Public,
                @event.Content);
        }
    }
}