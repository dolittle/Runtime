// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents committed aggregate events.
/// </summary>
/// <param name="EventSourceId">The event source of the aggregate root instance.</param>
/// <param name="AggregateRoot">The aggregate root type.</param>
/// <param name="AggregateRootVersion">The aggregate root version the events was applied by.</param>
/// <param name="Events">The committed events.</param>
public record CommittedAggregateEvents(
    string EventSourceId,
    Guid AggregateRoot,
    ulong AggregateRootVersion, 
    CommittedAggregateEvent[] Events)
{
    /// <summary>
    /// Converts a <see cref="Contracts.CommittedAggregateEvents"/> to a <see cref="CommittedAggregateEvents"/>.
    /// </summary>
    /// <param name="events">The events to convert.</param>
    /// <returns>The converted events.</returns>
    public static implicit operator CommittedAggregateEvents(Contracts.CommittedAggregateEvents events)
        => new(
            events.EventSourceId,
            events.AggregateRootId.ToGuid(),
            events.AggregateRootVersion,
            events.Events.Select(_ => (CommittedAggregateEvent) _).ToArray());
}
