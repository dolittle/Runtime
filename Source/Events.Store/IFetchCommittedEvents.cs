// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines an interface for fetching committed events the Event Store.
/// </summary>
public interface IFetchCommittedEvents
{
    /// <summary>
    /// Fetches all <see cref="CommittedAggregateEvent"/>s applied to an Event Source by an Aggregate Root.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="CommittedAggregateEvents"/> containing all <see cref="CommittedAggregateEvent"/> applied to the Event Source by the Aggregate root, in the order of which they appear in the Event Log.</returns>
    Task<CommittedAggregateEvents> FetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches all <see cref="CommittedAggregateEvent"/>s applied to an Event Source by an Aggregate Root after the given version.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
    /// <param name="after">The <see cref="AggregateRootVersion"/> to fetch events after.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="CommittedAggregateEvents"/> containing all <see cref="CommittedAggregateEvent"/> applied to the Event Source by the Aggregate root, in the order of which they appear in the Event Log.</returns>
    Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after, CancellationToken cancellationToken);
}