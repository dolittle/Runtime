// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that knows about <see cref="ICanFetchEventsFromStream">event fetchers</see>.
    /// </summary>
    public interface IEventFetchers
    {
        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventsFromStream" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventsFromStream" />.</returns>
        Task<ICanFetchEventsFromStream> GetFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventsFromPartitionedStream" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventsFromPartitionedStream" />.</returns>
        Task<ICanFetchEventsFromPartitionedStream> GetPartitionedFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventsFromPartitionedStream" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventsFromPartitionedStream" />.</returns>
        Task<ICanFetchEventsFromPartitionedStream> GetPublicEventsFetcherFor(IStreamDefinition streamDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an instance of <see cref="ICanFetchRangeOfEventsFromStream" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchRangeOfEventsFromStream" />.</returns>
        Task<ICanFetchRangeOfEventsFromStream> GetRangeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventTypesFromStream" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventTypesFromStream" />.</returns>
        Task<ICanFetchEventTypesFromStream> GetTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventTypesFromPartitionedStream" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventTypesFromPartitionedStream" />.</returns>
        Task<ICanFetchEventTypesFromPartitionedStream> GetPartitionedTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken);
    }
}
