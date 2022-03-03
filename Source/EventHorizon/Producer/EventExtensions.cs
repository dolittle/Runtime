// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Extension methods for <see cref="Contracts.EventHorizonEvent" />.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    /// Converts the <see cref="CommittedEvent" /> to <see cref="Events.Contracts.CommittedEvent" />.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent" />.</param>
    /// <returns>The <see cref="Events.Contracts.CommittedEvent" />.</returns>
    public static Events.Contracts.CommittedEvent ToCommittedEventHorizonEvent(this CommittedEvent @event)
    {
        var committedEvent = @event.ToProtobuf();
        committedEvent.ExecutionContext.Claims.Clear();
        committedEvent.ExecutionContext.Claims.AddRange(Claims.Empty.ToProtobuf());
        return committedEvent;
    }

    /// <summary>
    /// Converts the <see cref="StreamEvent" /> to <see cref="Contracts.EventHorizonEvent" />.
    /// </summary>
    /// <param name="event">The <see cref="StreamEvent" />.</param>
    /// <returns>The <see cref="Contracts.EventHorizonEvent" />.</returns>
    public static Contracts.EventHorizonEvent ToEventHorizonEvent(this StreamEvent @event) =>
        new()
        {
            Event = @event.Event.ToCommittedEventHorizonEvent(),
            StreamSequenceNumber = @event.Position
        };
}