// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents an set of uncommitted aggregate events for an aggregate root instance.
/// </summary>
/// <param name="EventSource">The event source of the aggregate root instance.</param>
/// <param name="AggregateRoot">The aggregate root type.</param>
/// <param name="AggregateRootVersion">The aggregate root version the events was applied by.</param>
/// <param name="Events">The events applied by the aggregate root.</param>
public record UncommittedAggregateEvents(
    string EventSource,
    Guid AggregateRoot,
    ulong AggregateRootVersion,
    UncommittedAggregateEvent[] Events)
{
    /// <summary>
    /// Converts an <see cref="UncommittedAggregateEvents"/> to an <see cref="Contracts.UncommittedAggregateEvents"/>.
    /// </summary>
    /// <param name="events">The events to convert.</param>
    /// <returns>The converted events.</returns>
    public static implicit operator Contracts.UncommittedAggregateEvents(UncommittedAggregateEvents events)
    {
        var converted = new Contracts.UncommittedAggregateEvents
        {
            EventSourceId = events.EventSource,
            AggregateRootId = events.AggregateRoot.ToProtobuf(),
            ExpectedAggregateRootVersion = events.AggregateRootVersion,
        };
        converted.Events.AddRange(events.Events.Select(_ => (Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent)_));
        return converted;
    }
}
