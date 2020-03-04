// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using Google.Protobuf.WellKnownTypes;
using grpcArtifacts = contracts::Dolittle.Runtime.Artifacts;
using grpcEvents = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for working with conversions between <see cref="CommittedAggregateEvent"/> and <see cref="grpcEvents.CommittedAggregateEvent"/>.
    /// </summary>
    public static class CommittedAggregateEventExtensions
    {
        /// <summary>
        /// Convert to a protobuf message representation of <see cref="CommittedAggregateEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="CommittedAggregateEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="grpcEvents.CommittedAggregateEvent"/>.</returns>
        public static grpcEvents.CommittedAggregateEvent ToProtobuf(this CommittedAggregateEvent @event)
        {
            return new grpcEvents.CommittedAggregateEvent
            {
                EventLogSequenceNumber = @event.EventLogSequenceNumber,
                Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
                AggregateRootVersion = @event.AggregateRootVersion,
                EventSource = @event.EventSource.ToProtobuf(),
                Correlation = @event.CorrelationId.ToProtobuf(),

                Microservice = @event.Microservice.ToProtobuf(),
                Tenant = @event.Tenant.ToProtobuf(),
                Cause = new grpcEvents.Cause
                {
                    Type = (int)@event.Cause.Type,
                    Position = @event.Cause.Position
                },
                Type = new grpcArtifacts.Artifact
                {
                    Id = @event.Type.Id.ToProtobuf(),
                    Generation = @event.Type.Generation
                },
                Content = @event.Content
            };
        }

        /// <summary>
        /// Convert to from <see cref="grpcEvents.CommittedAggregateEvent"/> to <see cref="CommittedAggregateEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="grpcEvents.CommittedAggregateEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="CommittedAggregateEvent"/>.</returns>
        public static CommittedEvent ToCommittedEvent(this grpcEvents.CommittedAggregateEvent @event)
        {
            return new CommittedAggregateEvent(
                @event.EventLogSequenceNumber,
                @event.AggregateRootVersion,
                @event.Occurred.ToDateTimeOffset(),
                @event.EventSource.To<EventSourceId>(),
                @event.Correlation.To<CorrelationId>(),
                @event.Microservice.To<Microservice>(),
                @event.Tenant.To<TenantId>(),
                new Cause((CauseType)@event.Cause.Type, @event.Cause.Position),
                new Artifact(@event.Type.Id.To<ArtifactId>(), @event.Type.Generation),
                @event.Public,
                @event.Content);
        }
    }
}