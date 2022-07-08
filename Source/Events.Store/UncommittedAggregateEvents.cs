// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents a sequence of events applied by an AggregateRoot to an Event Source that have not been committed to the Event Store.
/// </summary>
public class UncommittedAggregateEvents : UncommittedEvents
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UncommittedAggregateEvents"/> class.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> that the Events were applied to.</param>
    /// <param name="aggregateRoot">The <see cref="Artifact"/> representing the type of the Aggregate Root that applied the Event to the Event Source.</param>
    /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
    /// <param name="events">The <see cref="UncommittedEvent">events</see>.</param>
    public UncommittedAggregateEvents(EventSourceId eventSource, Artifact aggregateRoot, AggregateRootVersion expectedAggregateRootVersion, IReadOnlyList<UncommittedEvent> events)
        : base(events)
    {
        EventSource = eventSource;
        AggregateRoot = aggregateRoot;
        ExpectedAggregateRootVersion = expectedAggregateRootVersion;
    }

    /// <summary>
    /// Gets the Event Source that the uncommitted events was applied to.
    /// </summary>
    public EventSourceId EventSource { get; }

    /// <summary>
    /// Gets the <see cref="Artifact"/> representing the type of the Aggregate Root that applied the Event to the Event Source.
    /// </summary>
    public Artifact AggregateRoot { get; }

    /// <summary>
    /// Gets the <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.
    /// The events can only be committed to the Event Store if the version of Aggregate Root has not changed.
    /// </summary>
    public AggregateRootVersion ExpectedAggregateRootVersion { get; }
}