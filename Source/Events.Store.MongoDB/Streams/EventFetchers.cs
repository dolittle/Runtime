// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventFetchers" />.
    /// </summary>
    public class EventFetchers : IEventFetchers
    {
        readonly EventStoreConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFetchers"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        public EventFetchers(EventStoreConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<ICanFetchEventsFromStream> GetFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            if (streamDefinition.StreamId == StreamId.EventLog)
            {
                return await CreateStreamFetcherForEventLog(scopeId, cancellationToken).ConfigureAwait(false);
            }

            return await CreateStreamFetcherForStream(scopeId, streamDefinition.StreamId, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<ICanFetchEventsFromPartitionedStream> GetPartitionedFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            if (!streamDefinition.Partitioned) throw new CannotGetPartitionedFetcherForUnpartitionedStream(streamDefinition);
        }

        /// <inheritdoc/>
        public Task<ICanFetchRangeOfEventsFromStream> GetRangeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ICanFetchEventTypesFromStream> GetTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICanFetchEventsFromPartitionedStream> GetPublicEventsFetcherFor(IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ICanFetchEventTypesFromPartitionedStream> GetPartitionedTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        {
            if (!streamDefinition.Partitioned) throw new CannotGetPartitionedFetcherForUnpartitionedStream(streamDefinition);
        }

        async Task<StreamFetcher<MongoDB.Events.Event>> CreateStreamFetcherForEventLog(ScopeId scopeId, CancellationToken cancellationToken) =>
            new StreamFetcher<MongoDB.Events.Event>(
                await _connection.GetEventLogCollection(scopeId, cancellationToken).ConfigureAwait(false),
                Builders<MongoDB.Events.Event>.Filter,
                _ => _.EventLogSequenceNumber,
                Builders<MongoDB.Events.Event>.Projection.Expression(_ => _.ToRuntimeStreamEvent()),
                Builders<MongoDB.Events.Event>.Projection.Expression(_ => new Artifacts.Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration)));

        async Task<StreamFetcher<MongoDB.Events.StreamEvent>> CreateStreamFetcherForStream(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken) =>
            new StreamFetcher<MongoDB.Events.StreamEvent>(
                await _connection.GetStreamCollection(scopeId, streamId, cancellationToken).ConfigureAwait(false),
                Builders<MongoDB.Events.StreamEvent>.Filter,
                _ => _.StreamPosition,
                Builders<MongoDB.Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent()),
                Builders<MongoDB.Events.StreamEvent>.Projection.Expression(_ => new Artifacts.Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration)),
                _ => _.Partition);

    }
}
