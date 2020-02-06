// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Defines an interface for fetching committed events the Event Store.
    /// </summary>
    public interface IFetchCommittedEvents
    {
        /// <summary>
        /// Fetches all <see cref="CommittedAggregateEvent"/>s applied to an Event Source by an Aggregate Root />.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
        /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
        /// <returns>The <see cref="CommittedAggregateEvents"/> containing all <see cref="CommittedAggregateEvent"/>s applied to the Event Source by the Aggregate root, in the order of which they appear in the Event Log.</returns>
        CommittedAggregateEvents FetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot);
    }
}