// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Defines an interface for committing events.
    /// </summary>
    public interface ICommitEvents
    {
        /// <summary>
        /// Commits an <see cref="UncommittedEvents"/> to the Event Store, returning a corresponding <see cref="CommittedEvents"/>.
        /// </summary>
        /// <param name="events">The <see cref="UncommittedEvents"/> to be committed.</param>
        /// <returns><see cref="CommittedEvents"/> corresponding to the <see cref="UncommittedEvents"/> supplied.</returns>
        CommittedEvents CommitEvents(UncommittedEvents events);

        /// <summary>
        /// Commits an <see cref="UncommittedAggregateEvents"/> to the Event Store, returning a corresponding <see cref="CommittedAggregateEvents"/>.
        /// When committing event to the Event Store using Aggregate Roots, concurrency is guaranteed scoped to Aggregate Root instances.
        /// </summary>
        /// <param name="events">The <see cref="UncommittedAggregateEvents"/> to be committed.</param>
        /// <returns><see cref="CommittedAggregateEvents"/> corresponding to the <see cref="UncommittedAggregateEvents"/> supplied.</returns>
        CommittedAggregateEvents CommitAggregateEvents(UncommittedAggregateEvents events);
    }
}