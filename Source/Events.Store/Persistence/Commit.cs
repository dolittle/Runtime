// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Represents a commit containing events and aggregate events that can be written to persistent storage.
/// </summary>
/// <param name="Events">The events of the commit.</param>
/// <param name="AggregateEvents">The aggregate events of the commit.</param>
/// <param name="AllEvents">All committed events in order.</param>
public record Commit(
    IReadOnlyCollection<CommittedEvents> Events,
    IReadOnlyCollection<CommittedAggregateEvents> AggregateEvents,
    IReadOnlyCollection<CommittedEvent> AllEvents,
    EventLogSequenceNumber FromOffset,
    EventLogSequenceNumber ToOffset);
