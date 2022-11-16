// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents a sequence of events applied by an AggregateRoot to an Event Source that have been committed to the Event Store.
/// </summary>
public class CommittedAggregateEvents : CommittedEventSequence<CommittedAggregateEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedAggregateEvents"/> class.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> that the Events were applied to.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> representing the type of the Aggregate Root that applied the Event to the Event Source.</param>
    /// <param name="currentAggregateRootVersion">The current <see cref="AggregateRootVersion"/>.</param>
    /// <param name="events">The <see cref="CommittedAggregateEvent">events</see>.</param>
    public CommittedAggregateEvents(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion currentAggregateRootVersion, IReadOnlyList<CommittedAggregateEvent> events)
        : base(events)
    {
        EventSource = eventSource;
        AggregateRoot = aggregateRoot;
        AggregateRootVersion = currentAggregateRootVersion;
        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            ThrowIfEventWasAppliedToOtherEventSource(@event);
            ThrowIfEventWasAppliedByOtherAggregateRoot(@event);
            if (i > 0)
            {
                ThrowIfAggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, events[i-1].AggregateRootVersion);
            }
        }
    }

    /// <summary>
    /// Gets the Event Source that the Events were applied to.
    /// </summary>
    public EventSourceId EventSource { get; }

    /// <summary>
    /// Gets the <see cref="ArtifactId"/> representing the type of the Aggregate Root that applied the Event to the Event Source.
    /// </summary>
    public ArtifactId AggregateRoot { get; }
    
    /// <summary>
    /// Gets the current <see cref="AggregateRootVersion"/> of the aggregate root.
    /// <remarks>
    /// The number here will always be the number of events committed for an aggregate.
    /// However when this is converted to the CommittedAggregateEvents protobuf message it will populate now deprecated aggregateRootVersion field with the value of this minus 1.
    /// This is due to a mistake in how we first used the CommittedAggregateEvents protobuf message and the SDKs are built around this.
    /// To ensure backwards compatability we'll keep this flaw for now.
    /// </remarks>
    /// </summary>
    public AggregateRootVersion AggregateRootVersion { get; }

    void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvent @event)
    {
        if (@event.EventSource != EventSource)
        {
            throw new EventWasAppliedToOtherEventSource(AggregateRoot, @event.EventSource, EventSource);
        }
    }

    void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvent @event)
    {
        if (@event.AggregateRoot.Id != AggregateRoot)
        {
            throw new EventWasAppliedByOtherAggregateRoot(EventSource, @event.AggregateRoot.Id, AggregateRoot);
        }
    }

    void ThrowIfAggregateRootVersionIsOutOfOrder(AggregateRootVersion currentEventVersion, AggregateRootVersion previousEventVersion)
    {
        if (currentEventVersion <= previousEventVersion)
        {
            throw new AggregateRootVersionIsOutOfOrder(EventSource, AggregateRoot, currentEventVersion, previousEventVersion);
        }
    }
}
