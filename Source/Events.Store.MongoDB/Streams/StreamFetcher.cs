// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents a fetcher.
    /// </summary>
    /// <typeparam name="TEvent">The type of the stored event.</typeparam>
    public class StreamFetcher<TEvent> : ICanFetchEventsFromStream, ICanFetchEventsFromPartitionedStream, ICanFetchRangeOfEventsFromStream, ICanFetchEventTypesFromStream
        where TEvent : class
    {
        readonly IMongoCollection<TEvent> _stream;
        readonly FilterDefinitionBuilder<TEvent> _filter;
        readonly Expression<Func<TEvent, Guid>> _partitionIdExpression;
        readonly Expression<Func<TEvent, ulong>> _sequenceNumberExpression;
        readonly ProjectionDefinition<TEvent, Store.Streams.StreamEvent> _eventToStreamEvent;
        readonly ProjectionDefinition<TEvent, Artifact> _eventToArtifact;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFetcher{T}"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="partitionIdExpression">The <see cref="Expression{T}" /> for getting the <see cref="Guid" /> for the Partition Id from the stored event.</param>
        /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
        /// <param name="eventToStreamEvent">The <see cref="ProjectionDefinition{TSource, TProjection}" /> for projecting the stored event to a <see cref="Store.Streams.StreamEvent" />.</param>
        /// <param name="eventToArtifact">The <see cref="ProjectionDefinition{TSource, TProjection}" /> for projecting the stored event to <see cref="Artifact" />.</param>
        public StreamFetcher(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Expression<Func<TEvent, Guid>> partitionIdExpression,
            Expression<Func<TEvent, ulong>> sequenceNumberExpression,
            ProjectionDefinition<TEvent, Store.Streams.StreamEvent> eventToStreamEvent,
            ProjectionDefinition<TEvent, Artifact> eventToArtifact)
        {
            _stream = stream;
            _filter = filter;
            _partitionIdExpression = partitionIdExpression;
            _sequenceNumberExpression = sequenceNumberExpression;
            _eventToStreamEvent = eventToStreamEvent;
            _eventToArtifact = eventToArtifact;
        }

        /// <inheritdoc/>
        public async Task<Store.Streams.StreamEvent> Fetch(
            StreamPosition streamPosition,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _stream.Find(
                    _filter.Eq(_sequenceNumberExpression, streamPosition.Value))
                    .Project(_eventToStreamEvent)
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Store.Streams.StreamEvent> FetchInPartition(PartitionId partitionId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            try
            {
                return await _stream.Find(
                    _filter.Eq(_partitionIdExpression, partitionId.Value)
                        & _filter.Gte(_sequenceNumberExpression, streamPosition.Value))
                    .Project(_eventToStreamEvent)
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Store.Streams.StreamEvent>> FetchRange(
            StreamPositionRange range,
            CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = range.Length;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var events = await _stream.Find(
                        _filter.Gte(_sequenceNumberExpression, range.From.Value)
                            & _filter.Lte(_sequenceNumberExpression, range.From.Value + range.Length))
                    .Limit(limit)
                    .Project(_eventToStreamEvent)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                return events.ToArray();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchInRange(
            StreamPositionRange range,
            CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = range.Length;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                return await _stream
                    .Find(_filter.Gte(_sequenceNumberExpression, range.From.Value)
                        & _filter.Lte(_sequenceNumberExpression, range.From.Value + range.Length))
                    .Limit(limit)
                    .Project(_eventToArtifact)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchInRangeAndPartition(PartitionId partitionId, StreamPositionRange range, CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = range.Length;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                return await _stream
                    .Find(_filter.Eq(_partitionIdExpression, partitionId.Value)
                        & _filter.Gte(_sequenceNumberExpression, range.From.Value)
                        & _filter.Lte(_sequenceNumberExpression, range.From.Value + range.Length))
                    .Limit(limit)
                    .Project(_eventToArtifact)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}
