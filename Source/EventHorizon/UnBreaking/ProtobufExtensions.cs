// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.EventHorizon.UnBreaking;

/// <summary>
/// Extension methods for protobuf messages for un breaking event horizon change.
/// </summary>
public static class ProtobufExtensions
{
    public static PartitionId GetPartitionId(this Contracts.ConsumerSubscriptionRequest req)
        => req.PartitionId != default ? req.PartitionId.ToGuid().ToString() : req.PartitionIdString;

    /// <summary>
    /// Converts the <see cref="CommittedEvent" /> to <see cref="Contracts.EventHorizonCommittedEvent" />.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent" />.</param>
    /// <returns>The <see cref="Contracts.EventHorizonCommittedEvent" />.</returns>
    public static Contracts.EventHorizonCommittedEvent ToEventHorizonCommittedEvent(this CommittedEvent @event)
    {
        var committedEvent = new Contracts.EventHorizonCommittedEvent
        {
            EventLogSequenceNumber = @event.EventLogSequenceNumber,
            Occurred = Timestamp.FromDateTimeOffset(@event.Occurred),
            EventSourceIdString = @event.EventSource.Value,
            ExecutionContext = @event.ExecutionContext.ToProtobuf(),
            EventType = new Dolittle.Artifacts.Contracts.Artifact
            {
                Id = @event.Type.Id.ToProtobuf(),
                Generation = @event.Type.Generation
            },
            Content = @event.Content,
            Public = @event.Public,
        };
        if (Guid.TryParse(@event.EventSource, out var eventSourceGuid))
        {
            committedEvent.EventSourceId = eventSourceGuid.ToProtobuf();
        }
        committedEvent.ExecutionContext.Claims.Clear();
        committedEvent.ExecutionContext.Claims.AddRange(Claims.Empty.ToProtobuf());
        return committedEvent;
    }
    /// <summary>
    /// Convert to from <see cref="Contracts.EventHorizonCommittedEvent"/> to <see cref="CommittedEvent"/>.
    /// </summary>
    /// <param name="event"><see cref="Contracts.EventHorizonCommittedEvent"/> to convert from.</param>
    /// <returns>Converted <see cref="CommittedEvent"/>.</returns>
    public static CommittedEvent ToCommittedEvent(this Contracts.EventHorizonCommittedEvent @event) =>
        new(
            @event.EventLogSequenceNumber,
            @event.Occurred.ToDateTimeOffset(),
            @event.EventSourceId != default ? @event.EventSourceId.ToGuid().ToString() : @event.EventSourceIdString,
            @event.ExecutionContext.ToExecutionContext(),
            new Artifact(@event.EventType.Id.ToGuid(), @event.EventType.Generation),
            @event.Public,
            @event.Content);
}
