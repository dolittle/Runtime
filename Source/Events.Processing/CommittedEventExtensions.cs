// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Tenancy;
using Google.Protobuf.WellKnownTypes;
using grpc = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Extensions for working with conversions between <see cref="Store.CommittedEvent"/> and <see cref="grpc.CommittedEvent"/>.
    /// </summary>
    public static class CommittedEventExtensions
    {
        /// <summary>
        /// Convert to a protobuf message representation of <see cref="Store.CommittedEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="Store.CommittedEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="grpc.CommittedEvent"/>.</returns>
        public static grpc.CommittedEvent ToProtobuf(this Store.CommittedEvent @event)
        {
            return new grpc.CommittedEvent
            {
                EventLogVersion = @event.EventLogVersion,
                Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
                CorrelationId = @event.CorrelationId.ToProtobuf(),
                Microservice = @event.Microservice.ToProtobuf(),
                Tenant = @event.Tenant.ToProtobuf(),
                Cause = new grpc.Cause
                {
                    Type = (int)@event.Cause.Type,
                    Position = @event.Cause.Position
                },
                Type = new grpc.Artifact
                {
                    Id = @event.Type.Id.ToProtobuf(),
                    Generation = @event.Type.Generation
                },
                Content = @event.Content
            };
        }

        /// <summary>
        /// Convert to from <see cref="grpc.CommittedEvent"/> to <see cref="Store.CommittedEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="grpc.CommittedEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="Store.CommittedEvent"/>.</returns>
        public static Store.CommittedEvent ToCommittedEvent(this grpc.CommittedEvent @event)
        {
            return new Store.CommittedEvent(
                @event.EventLogVersion,
                @event.Occurred.ToDateTimeOffset(),
                @event.CorrelationId.To<CorrelationId>(),
                @event.Microservice.To<Microservice>(),
                @event.Tenant.To<TenantId>(),
                new Cause((CauseType)@event.Cause.Type, @event.Cause.Position),
                new Artifact(@event.Type.Id.To<ArtifactId>(), @event.Type.Generation),
                @event.Content);
        }
    }
}