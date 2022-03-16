// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.EventHorizon.UnBreaking;

/// <summary>
/// Extension methods for protobufd messages .
/// </summary>
public static class ProtobufExtensions
{
    public static PartitionId GetPartitionId(this Contracts.ConsumerSubscriptionRequest req)
        => req.PartitionId != default ? req.PartitionId.ToGuid().ToString() : req.PartitionIdString;

    /// <summary>
    /// Converts the <see cref="CommittedEvent" /> to <see cref="Events.Contracts.CommittedEvent" />.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent" />.</param>
    /// <returns>The <see cref="Events.Contracts.CommittedEvent" />.</returns>
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
}
