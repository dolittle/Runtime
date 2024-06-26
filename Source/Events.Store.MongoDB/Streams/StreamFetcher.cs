// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;
using System.Linq;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.MongoDB;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents a fetcher.
/// </summary>
/// <typeparam name="TEvent">The type of the stored event.</typeparam>
public class StreamFetcher<TEvent> : ICanFetchEventsFromStream, ICanFetchEventsFromPartitionedStream, ICanFetchRangeOfEventsFromStream,
    ICanFetchEventTypesFromStream, ICanFetchEventTypesFromPartitionedStream
    where TEvent : class
{
    const int FetchEventsBatchSize = 100;

    readonly StreamId _stream;
    readonly ScopeId _scope;
    readonly IMongoCollection<TEvent> _collection;
    readonly FilterDefinitionBuilder<TEvent> _filter;
    readonly Expression<Func<TEvent, ulong>> _sequenceNumberExpression;
    readonly SortDefinition<TEvent> _sequenceNumberSortByExpressionAsc;
    readonly SortDefinition<TEvent> _sequenceNumberSortByExpressionDesc;
    readonly Func<TEvent, StreamEvent> _eventToStreamEvent;
    readonly Expression<Func<TEvent, Guid>> _eventToArtifactId;
    readonly Expression<Func<TEvent, uint>> _eventToArtifactGeneration;
    readonly Expression<Func<TEvent, string>> _partitionIdExpression = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamFetcher{T}"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="StreamId" />.</param>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="collection">The <see cref="IMongoCollection{TDocument}" />.</param>
    /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
    /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
    /// <param name="eventToStreamEvent">The <see cref="Expression{T}" /> for projecting the stored event to a <see cref="StreamEvent" />.</param>
    /// <param name="eventToArtifact">The <see cref="Expression{T}" /> for projecting the stored event to <see cref="Artifact" />.</param>
    /// <param name="partitionIdExpression">The <see cref="Expression{T}" /> for getting the <see cref="Guid" /> for the Partition Id from the stored event.</param>
    public StreamFetcher(
        StreamId stream,
        ScopeId scope,
        IMongoCollection<TEvent> collection,
        FilterDefinitionBuilder<TEvent> filter,
        Expression<Func<TEvent, ulong>> sequenceNumberExpression,
        Expression<Func<TEvent, object>> sequenceNumberSortByExpression,
        Expression<Func<TEvent, StreamEvent>> eventToStreamEvent,
        Expression<Func<TEvent, Guid>> eventToArtifactId,
        Expression<Func<TEvent, uint>> eventToArtifactGeneration)
    {
        _stream = stream;
        _scope = scope;
        _collection = collection;
        _filter = filter;
        _sequenceNumberExpression = sequenceNumberExpression;
        _sequenceNumberSortByExpressionAsc = Builders<TEvent>.Sort.Ascending(sequenceNumberSortByExpression);
        _sequenceNumberSortByExpressionDesc = Builders<TEvent>.Sort.Descending(sequenceNumberSortByExpression);
        _eventToStreamEvent = eventToStreamEvent.Compile();

        _eventToArtifactId = eventToArtifactId;
        _eventToArtifactGeneration = eventToArtifactGeneration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamFetcher{T}"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="StreamId" />.</param>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="collection">The <see cref="IMongoCollection{TDocument}" />.</param>
    /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
    /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
    /// <param name="sequenceNumberSortByExpression"></param>
    /// <param name="eventToStreamEvent">The <see cref="Expression{T}" /> for projecting the stored event to a <see cref="StreamEvent" />.</param>
    /// <param name="eventToArtifact">The <see cref="Expression{T}" /> for projecting the stored event to <see cref="Artifact" />.</param>
    /// <param name="partitionIdExpression">The <see cref="Expression{T}" /> for getting the <see cref="Guid" /> for the Partition Id from the stored event.</param>
    public StreamFetcher(
        StreamId stream,
        ScopeId scope,
        IMongoCollection<TEvent> collection,
        FilterDefinitionBuilder<TEvent> filter,
        Expression<Func<TEvent, ulong>> sequenceNumberExpression,
        Expression<Func<TEvent, object>> sequenceNumberSortByExpression,
        Expression<Func<TEvent, StreamEvent>> eventToStreamEvent,
        Expression<Func<TEvent, Guid>> eventToArtifactId,
        Expression<Func<TEvent, uint>> eventToArtifactGeneration,
        Expression<Func<TEvent, string>> partitionIdExpression)
        : this(stream, scope, collection, filter, sequenceNumberExpression, sequenceNumberSortByExpression, eventToStreamEvent, eventToArtifactId,
            eventToArtifactGeneration)
    {
        _partitionIdExpression = partitionIdExpression;
    }

    /// <inheritdoc/>
    public async Task<Try<IEnumerable<StreamEvent>>> Fetch(StreamPosition position, CancellationToken cancellationToken)
    {
        try
        {
            var results = await _collection.Find(_filter.Gte(_sequenceNumberExpression, position.Value))
                .Sort(_sequenceNumberSortByExpressionAsc)
                .Limit(FetchEventsBatchSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            var events = results.Select(_eventToStreamEvent).ToList();

            return events == default || events.Count == 0
                ? new NoEventInStreamAtPosition(_stream, _scope, position)
                : events;
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Try<StreamEvent>> FetchSingle(StreamPosition position, CancellationToken cancellationToken)
    {
        try
        {
            var events = await _collection.Find(_filter.Eq(_sequenceNumberExpression, position.Value))
                .SingleOrDefaultAsync(cancellationToken);

            return events is null
                ? new NoEventInStreamAtPosition(_stream, _scope, position)
                : _eventToStreamEvent(events);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public async Task<Try<StreamEvent>> FetchLast(CancellationToken cancellationToken)
    {
        try
        {
            var events = await _collection.Find(_filter.Empty)
                .Sort(_sequenceNumberSortByExpressionDesc)
                .FirstOrDefaultAsync(cancellationToken);

            return events is null
                ? new NoEventInStreamAtPosition(_stream, _scope, StreamPosition.Start)
                : _eventToStreamEvent(events);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Try<StreamPosition>> GetNextStreamPosition(CancellationToken cancellationToken)
    {
        try
        {
            var last = await FetchLast(cancellationToken);
            if (!last.Success)
            {
                return Try<StreamPosition>.Succeeded(StreamPosition.Start);
            }

            return last.Result.Position.Increment();
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Try<IEnumerable<StreamEvent>>> FetchInPartition(PartitionId partitionId, StreamPosition position, CancellationToken cancellationToken)
    {
        ThrowIfNotConstructedWithPartitionIdExpression();
        try
        {
            var results = await _collection.Find(
                    _filter.EqStringOrGuid(_partitionIdExpression, partitionId.Value)
                    & _filter.Gte(_sequenceNumberExpression, position.Value))
                .Sort(_sequenceNumberSortByExpressionAsc)
                .Limit(FetchEventsBatchSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            var events = results.Select(_eventToStreamEvent).ToList();

            return events == default || events.Count == 0
                ? new NoEventInPartitionInStreamFromPosition(_stream, _scope, partitionId, position)
                : events;
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public async Task<(IList<StreamEvent> events, bool hasMoreEvents)> FetchInPartition(PartitionId partitionId, StreamPosition from, StreamPosition to,
        ISet<Guid> artifactIds,
        CancellationToken cancellationToken)
    {
        ThrowIfNotConstructedWithPartitionIdExpression();
        try
        {
            var composedFilter = _filter.EqStringOrGuid(_partitionIdExpression, partitionId.Value)
                                 & _filter.Gte(_sequenceNumberExpression, from.Value)
                                 & _filter.Lt(_sequenceNumberExpression, to.Value);
            
            if (artifactIds.Any())
            {
                composedFilter &= _filter.In(_eventToArtifactId, artifactIds);
            }

            var results = await _collection.Find(composedFilter)
                .Sort(_sequenceNumberSortByExpressionAsc)
                .Limit(FetchEventsBatchSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            var events = results.Select(_eventToStreamEvent).ToList();

            return (events, events.Count < FetchEventsBatchSize);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public async Task<(StreamEvent? events, bool hasMoreEvents)> FetchNextEventInPartition(PartitionId partitionId, StreamPosition from, StreamPosition to,
        ISet<Guid> artifactIds,
        CancellationToken cancellationToken)
    {
        ThrowIfNotConstructedWithPartitionIdExpression();
        try
        {
            var composedFilter = _filter.EqStringOrGuid(_partitionIdExpression, partitionId.Value)
                                 & _filter.Gte(_sequenceNumberExpression, from.Value)
                                 & _filter.Lt(_sequenceNumberExpression, to.Value);
            if (artifactIds.Any())
            {
                composedFilter &= _filter.In(_eventToArtifactId, artifactIds);
            }

            var results = await _collection.Find(composedFilter)
                .Sort(_sequenceNumberSortByExpressionAsc)
                .Limit(2)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            if (results.Count == 0)
                return (null, false);

            return results.Count switch
            {
                0 => (null, false),
                1 => (_eventToStreamEvent(results[0]), false),
                _ => (_eventToStreamEvent(results[0]), true)
            };
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamEvent> FetchRange(
        StreamPositionRange range,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = _collection.Find(
                    _filter.Gte(_sequenceNumberExpression, range.From.Value)
                    & _filter.Lt(_sequenceNumberExpression, range.From.Value + range.Length))
                .Sort(_sequenceNumberSortByExpressionAsc)
                .ToAsyncEnumerable(cancellationToken);
            return results.Select(_eventToStreamEvent);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc/>
    public Task<ISet<Artifact>> FetchInRange(StreamPositionRange range, CancellationToken cancellationToken)
        => FetchTypesWithFilter(
            _filter.Gte(_sequenceNumberExpression, range.From.Value)
            & _filter.Lt(_sequenceNumberExpression, range.From.Value + range.Length),
            cancellationToken);

    /// <inheritdoc/>
    public async Task<ISet<Artifact>> FetchInRangeAndPartition(PartitionId partitionId, StreamPositionRange range, CancellationToken cancellationToken)
    {
        ThrowIfNotConstructedWithPartitionIdExpression();
        return await FetchTypesWithFilter(
            _filter.EqStringOrGuid(_partitionIdExpression, partitionId.Value)
            & _filter.Gte(_sequenceNumberExpression, range.From.Value)
            & _filter.Lt(_sequenceNumberExpression, range.From.Value + range.Length),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<ISet<Artifact>> FetchTypesWithFilter(FilterDefinition<TEvent> filter, CancellationToken cancellationToken)
    {
        try
        {
            var typesWithGenerations = await _collection
                .Aggregate()
                .Match(filter)
                .Group(_eventToArtifactId, _ => new ArtifactWithGenerations(_.Key, _.AsQueryable().Select(_eventToArtifactGeneration).Distinct()))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return ExpandToArtifacts(typesWithGenerations);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    static ISet<Artifact> ExpandToArtifacts(IEnumerable<ArtifactWithGenerations> artifactsWithGenerations)
    {
        var set = new HashSet<Artifact>();
        foreach (var artifactWithGenerations in artifactsWithGenerations)
        {
            foreach (var generation in artifactWithGenerations.Generations)
            {
                set.Add(new Artifact(artifactWithGenerations.Id, generation));
            }
        }

        return set;
    }

    class ArtifactWithGenerations
    {
        public ArtifactWithGenerations(Guid id, IEnumerable<uint> generations)
        {
            Id = id;
            Generations = generations;
        }

        public Guid Id { get; set; }

        public IEnumerable<uint> Generations { get; set; }
    }

    void ThrowIfNotConstructedWithPartitionIdExpression()
    {
        if (_partitionIdExpression == default)
        {
            throw new StreamFetcherWasNotConstructedWithPartitionIdExpression();
        }
    }
}
