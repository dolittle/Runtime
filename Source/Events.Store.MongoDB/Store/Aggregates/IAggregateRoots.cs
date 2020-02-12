// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Defines a system that is capable of ensuring consistency with optimistic concurrency for aggregate root versions in the event store.
    /// </summary>
    public interface IAggregateRoots
    {
        /// <summary>
        /// Increments the version of the aggregate root instance in the event store.
        /// </summary>
        /// <param name="transaction">The <see cref="IClientSessionHandle" />.</param>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRoot">The <see cref="ArtifactId" />.</param>
        /// <param name="expectedVersion">The <see cref="AggregateRootVersion" />.</param>
        /// <param name="nextVersion">The new version of the aggregate root instance to persist.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The new <see cref="AggregateRoot" />.</returns>
        Task<AggregateRoot> IncrementVersionFor(
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            AggregateRootVersion expectedVersion,
            AggregateRootVersion nextVersion,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the current version of the aggregate root instance in the event store.
        /// </summary>
        /// <param name="transaction">The <see cref="IClientSessionHandle" />.</param>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRoot">The <see cref="ArtifactId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The current <see cref="AggregateRootVersion" /> for an aggregate root instance.</returns>
        Task<AggregateRootVersion> FetchVersionFor(
            IClientSessionHandle transaction,
            EventSourceId eventSource,
            ArtifactId aggregateRoot,
            CancellationToken cancellationToken = default);
    }
}