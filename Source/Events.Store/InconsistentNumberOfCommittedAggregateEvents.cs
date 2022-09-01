// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when there is an inconsistent number of committed aggregate events for an aggregate root. Meaning that it is more or less aggregate events than the current <see cref="AggregateRootVersion"/>.
/// </summary>s
public class InconsistentNumberOfCommittedAggregateEvents : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InconsistentNumberOfCommittedAggregateEvents"/> class.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
    /// <param name="aggregateRoot">The aggregate root id.</param>
    /// <param name="aggregateRootVersion">The current <see cref="AggregateRootVersion"/>.</param>
    /// <param name="numEvents">The number of events.</param>
    public InconsistentNumberOfCommittedAggregateEvents(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion aggregateRootVersion, int numEvents)
        : base($"Aggregate Root version for event source {eventSource} on aggregate root {aggregateRoot} is at version {aggregateRootVersion} but only {numEvents} aggregate events was found.")
    {
    }
}
