// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for working with conversions between <see cref="CommittedExternalEvent"/> and <see cref="Contracts.CommittedEvent"/>.
    /// </summary>
    public static class CommittedExternalEventExtensions
    {
        /// <summary>
        /// Convert to a protobuf message representation of <see cref="CommittedExternalEvent"/>.
        /// </summary>
        /// <param name="event"><see cref="CommittedExternalEvent"/> to convert from.</param>
        /// <returns>Converted <see cref="Contracts.CommittedEvent"/>.</returns>
        public static Contracts.CommittedEvent ToProtobuf(this CommittedExternalEvent @event) =>
            new Contracts.CommittedEvent
                {
                    EventLogSequenceNumber = @event.EventLogSequenceNumber,
                    Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
                    EventSourceId = @event.EventSource.ToProtobuf(),
                    ExecutionContext = @event.ExecutionContext.ToProtobuf(),
                    Type = new Artifacts.Contracts.Artifact
                    {
                        Id = @event.Type.Id.ToProtobuf(),
                        Generation = @event.Type.Generation
                    },
                    Content = @event.Content,
                    Public = @event.Public,
                    External = true,
                    ExternalEventLogSequenceNumber = @event.ExternalEventLogSequenceNumber,
                    ExternalEventReceived = Timestamp.FromDateTimeOffset(@event.Received)
                };
    }
}
