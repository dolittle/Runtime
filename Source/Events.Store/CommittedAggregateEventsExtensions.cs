// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Extension methods for <see cref="CommittedAggregateEvents" />.
/// </summary>
public static class CommittedAggregateEventsExtensions
{
    /// <summary>
    /// Converts the <see cref="CommittedAggregateEvents" /> to <see cref="Contracts.CommittedAggregateEvents" />s.
    /// </summary>
    /// <param name="committedAggregateEvents">The committed events.</param>
    /// <returns>The converted <see cref="Contracts.CommittedAggregateEvents" />.</returns>
    public static Contracts.CommittedAggregateEvents ToProtobuf(this CommittedAggregateEvents committedAggregateEvents)
    {
        var aggregateRootVersion = committedAggregateEvents.AsEnumerable().LastOrDefault()?.AggregateRootVersion ?? 0;
        var protobuf = new Contracts.CommittedAggregateEvents
        {
            AggregateRootId = committedAggregateEvents.AggregateRoot.ToProtobuf(),
            EventSourceId = committedAggregateEvents.EventSource.Value,
            AggregateRootVersion = aggregateRootVersion
        };
        protobuf.Events.AddRange(committedAggregateEvents.Select(_ => _.ToProtobuf()));
        return protobuf;
    }

    public static CommittedAggregateEvents ToCommittedEvents(this Contracts.CommittedAggregateEvents committedAggregateEvents)
    {
        var version = committedAggregateEvents.AggregateRootVersion - (ulong)committedAggregateEvents.Events.Count;
        return new CommittedAggregateEvents(
            committedAggregateEvents.EventSourceId,
            committedAggregateEvents.AggregateRootId.ToGuid(),
            committedAggregateEvents.Events.Select(_ => new CommittedAggregateEvent(
                new Artifact(committedAggregateEvents.AggregateRootId.ToGuid(), ArtifactGeneration.First),
                version++,
                _.EventLogSequenceNumber,
                _.Occurred.ToDateTimeOffset(),
                committedAggregateEvents.EventSourceId,
                _.ExecutionContext.ToExecutionContext(),
                _.EventType.ToArtifact(),
                _.Public,
                _.Content)).ToList());
    }
}
