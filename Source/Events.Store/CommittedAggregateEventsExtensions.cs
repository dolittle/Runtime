// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
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
}