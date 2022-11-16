// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;
using ArtifactsContracts = Dolittle.Artifacts.Contracts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Extensions for working with conversions between <see cref="CommittedAggregateEvent"/> and <see cref="Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent"/>.
/// </summary>
public static class CommittedAggregateEventExtensions
{
    /// <summary>
    /// Convert to a protobuf message representation of <see cref="CommittedAggregateEvent"/>.
    /// </summary>
    /// <param name="event"><see cref="CommittedAggregateEvent"/> to convert from.</param>
    /// <returns>Converted <see cref="Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent"/>.</returns>
    public static Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent ToProtobuf(this CommittedAggregateEvent @event)
        => new()
        {
            EventLogSequenceNumber = @event.EventLogSequenceNumber,
            Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
            ExecutionContext = @event.ExecutionContext.ToProtobuf(),
            EventType = new ArtifactsContracts.Artifact
            {
                Id = @event.Type.Id.ToProtobuf(),
                Generation = @event.Type.Generation
            },
            Content = @event.Content,
            Public = @event.Public,
            AggregateRootVersion = @event.AggregateRootVersion
        };
}
