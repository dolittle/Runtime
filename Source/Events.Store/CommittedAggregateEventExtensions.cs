// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Google.Protobuf.WellKnownTypes;
using grpcArtifacts = contracts::Dolittle.Artifacts.Contracts;
using grpcEvents = contracts::Dolittle.Runtime.Events.Contracts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for working with conversions between <see cref="CommittedAggregateEvent"/> and <see cref="grpcEvents.CommittedAggregateEvents.Types.CommittedAggregateEvent"/>.
    /// </summary>
    public static class CommittedAggregateEventExtensions
    {
        /// <summary>
        /// Convert to a protobuf message representation of <see cref="CommittedAggregateEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="CommittedAggregateEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="grpcEvents.CommittedAggregateEvents.Types.CommittedAggregateEvent"/>.</returns>
        public static grpcEvents.CommittedAggregateEvents.Types.CommittedAggregateEvent ToProtobuf(this CommittedAggregateEvent @event)
        {
            return new grpcEvents.CommittedAggregateEvents.Types.CommittedAggregateEvent
            {
                EventLogSequenceNumber = @event.EventLogSequenceNumber,
                Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
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
    }
}