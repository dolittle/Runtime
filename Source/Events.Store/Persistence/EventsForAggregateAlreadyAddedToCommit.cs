// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Exception that gets thrown when there are multiple events added to a <see cref="Commit"/> for the same aggregate.
/// </summary>
public class EventsForAggregateAlreadyAddedToCommit : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventsForAggregateAlreadyAddedToCommit"/> class.
    /// </summary>
    /// <param name="aggregate"></param>
    public EventsForAggregateAlreadyAddedToCommit(Aggregate aggregate)
        : base($"There are already events for aggregate with root type {aggregate.AggregateRoot} and event source {aggregate.EventSourceId}")
    {
    }
}
