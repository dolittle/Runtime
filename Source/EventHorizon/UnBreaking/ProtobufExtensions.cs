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
    /// <summary>
    /// Gets the <see cref="PartitionId"/> from a <see cref="Contracts.ConsumerSubscriptionRequest"/>.
    /// </summary>
    /// <param name="req">The <see cref="Contracts.ConsumerSubscriptionRequest"/> to get the.</param>
    /// <returns>The <see cref="PartitionId"/> from the request.</returns>
    public static PartitionId GetPartitionId(this Contracts.ConsumerSubscriptionRequest req)
        => req.PartitionId ?? req.PartitionIdLegacy.ToGuid().ToString();
    
    /// <summary>
    /// Tries to set the legacy partition id guid field on the <see cref="Contracts.ConsumerSubscriptionRequest"/> if the partition id is a guid.
    /// </summary>
    /// <param name="req">The <see cref="Contracts.ConsumerSubscriptionRequest"/>.</param>
    public static void TrySetPartitionIdLegacy(this Contracts.ConsumerSubscriptionRequest req)
    {
        if (Guid.TryParse(req.PartitionId, out var partitionGuid))
        {
            req.PartitionIdLegacy = partitionGuid.ToProtobuf();
        }
    }

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
            EventSourceId = @event.EventSource.Value,
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
            committedEvent.EventSourceIdLegacy = eventSourceGuid.ToProtobuf();
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
            @event.EventSourceId ?? @event.EventSourceIdLegacy.ToGuid().ToString(),
            @event.ExecutionContext.ToExecutionContext(),
            new Artifact(@event.EventType.Id.ToGuid(), @event.EventType.Generation),
            @event.Public,
            @event.Content);
}
