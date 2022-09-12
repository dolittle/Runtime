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
public class StreamFetcher<TEvent> : ICanFetchEventsFromStream, ICanFetchEventsFromPartitionedStream, ICanFetchRangeOfEventsFromStream, ICanFetchEventTypesFromStream, ICanFetchEventTypesFromPartitionedStream
    where TEvent : class
{
    const int FetchEventsBatchSize = 100;

    readonly StreamId _stream;
    readonly ScopeId _scope;
    readonly IMongoCollection<TEvent> _collection;
    readonly FilterDefinitionBuilder<TEvent> _filter;
    readonly Expression<Func<TEvent, ulong>> _sequenceNumberExpression;
    readonly Expression<Func<TEvent, StreamEvent>> _eventToStreamEvent;
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
        Expression<Func<TEvent, StreamEvent>> eventToStreamEvent,
        Expression<Func<TEvent, Guid>> eventToArtifactId,
        Expression<Func<TEvent, uint>> eventToArtifactGeneration)
    {
        _stream = stream;
        _scope = scope;
        _collection = collection;
        _filter = filter;
        _sequenceNumberExpression = sequenceNumberExpression;
        _eventToStreamEvent = eventToStreamEvent;
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
    /// <param name="eventToStreamEvent">The <see cref="Expression{T}" /> for projecting the stored event to a <see cref="StreamEvent" />.</param>
    /// <param name="eventToArtifact">The <see cref="Expression{T}" /> for projecting the stored event to <see cref="Artifact" />.</param>
    /// <param name="partitionIdExpression">The <see cref="Expression{T}" /> for getting the <see cref="Guid" /> for the Partition Id from the stored event.</param>
    public StreamFetcher(
        StreamId stream,
        ScopeId scope,
        IMongoCollection<TEvent> collection,
        FilterDefinitionBuilder<TEvent> filter,
        Expression<Func<TEvent, ulong>> sequenceNumberExpression,
        Expression<Func<TEvent, StreamEvent>> eventToStreamEvent,
        Expression<Func<TEvent, Guid>> eventToArtifactId,
        Expression<Func<TEvent, uint>> eventToArtifactGeneration,
        Expression<Func<TEvent, string>> partitionIdExpression)
        : this(stream, scope, collection, filter, sequenceNumberExpression, eventToStreamEvent, eventToArtifactId, eventToArtifactGeneration)
    {
        _partitionIdExpression = partitionIdExpression;
    }

    /// <inheritdoc/>
    public async Task<Try<IEnumerable<StreamEvent>>> Fetch(StreamPosition streamPosition, CancellationToken cancellationToken)
    {
        try
        {
            var events = await _collection.Find(
                    _filter.Gte(_sequenceNumberExpression, streamPosition.Value))
                .Limit(FetchEventsBatchSize)
                .Project(_eventToStreamEvent)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return events == default || events.Count == 0
                ? new NoEventInStreamAtPosition(_stream, _scope, streamPosition)
                : events;
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
            // TODO: Do not do this, get last document instead.
            var numEvents = await _collection.CountDocumentsAsync(_filter.Empty, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Try<StreamPosition>.Succeeded((ulong)numEvents);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Try<IEnumerable<StreamEvent>>> FetchInPartition(PartitionId partitionId, StreamPosition streamPosition, CancellationToken cancellationToken)
    {
        ThrowIfNotConstructedWithPartitionIdExpression();
        try
        {
            var events = await _collection.Find(
                    _filter.EqStringOrGuid(_partitionIdExpression, partitionId.Value)
                    & _filter.Gte(_sequenceNumberExpression, streamPosition.Value))
                .Limit(FetchEventsBatchSize)
                .Project(_eventToStreamEvent)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return events == default || events.Count == 0
                ? new NoEventInPartitionInStreamFromPosition(_stream, _scope, partitionId, streamPosition)
                : events;
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
            return _collection.Find(
                    _filter.Gte(_sequenceNumberExpression, range.From.Value)
                    & _filter.Lt(_sequenceNumberExpression, range.From.Value + range.Length))
                .Project(_eventToStreamEvent)
                .ToAsyncEnumerable(cancellationToken);
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
