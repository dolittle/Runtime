// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents an implementation of <see cref="IEventFetchers" />.
/// </summary>
public class EventFetchers : IEventFetchers
{
    readonly IStreams _streams;
    readonly IEventConverter _eventConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventFetchers"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams" />.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    public EventFetchers(IStreams streams, IEventConverter eventConverter)
    {
        _streams = streams;
        _eventConverter = eventConverter;
    }

    /// <inheritdoc/>
    public async Task<ICanFetchEventsFromStream> GetFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
    {
        var stream = streamDefinition.StreamId;
        if (stream == StreamId.EventLog)
        {
            return await CreateStreamFetcherForEventLog(scopeId, cancellationToken).ConfigureAwait(false);
        }

        if (streamDefinition.Public)
        {
            return CreateStreamFetcherForStreamEventCollection(
                await _streams.GetPublic(stream, cancellationToken).ConfigureAwait(false),
                stream,
                scopeId,
                streamDefinition.Partitioned);
        }

        return CreateStreamFetcherForStreamEventCollection(
            await _streams.Get(scopeId, stream, cancellationToken).ConfigureAwait(false),
            stream,
            scopeId,
            streamDefinition.Partitioned);
    }

    /// <inheritdoc/>
    public async Task<ICanFetchEventsFromPartitionedStream> GetPartitionedFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
    {
        if (!streamDefinition.Partitioned) throw new CannotGetPartitionedFetcherForUnpartitionedStream(streamDefinition);
        if (streamDefinition.StreamId == StreamId.EventLog) throw new CannotGetPartitionedFetcherForEventLog();
        if (streamDefinition.Public)
        {
            return CreateStreamFetcherForStreamEventCollection(
                await _streams.GetPublic(streamDefinition.StreamId, cancellationToken).ConfigureAwait(false),
                streamDefinition.StreamId,
                scopeId,
                streamDefinition.Partitioned);
        }

        return CreateStreamFetcherForStreamEventCollection(
            await _streams.Get(scopeId, streamDefinition.StreamId, cancellationToken).ConfigureAwait(false),
            streamDefinition.StreamId,
            scopeId,
            streamDefinition.Partitioned);
    }

    /// <inheritdoc/>
    public async Task<ICanFetchRangeOfEventsFromStream> GetRangeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
    {
        return await GetFetcherFor(scopeId, streamDefinition, cancellationToken).ConfigureAwait(false) as ICanFetchRangeOfEventsFromStream;
    }

    /// <inheritdoc/>
    public async Task<ICanFetchEventTypesFromStream> GetTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
    {
        return await GetFetcherFor(scopeId, streamDefinition, cancellationToken).ConfigureAwait(false) as ICanFetchEventTypesFromStream;
    }

    /// <inheritdoc/>
    public async Task<ICanFetchEventTypesFromPartitionedStream> GetPartitionedTypeFetcherFor(ScopeId scopeId, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
    {
        return await GetPartitionedFetcherFor(scopeId, streamDefinition, cancellationToken).ConfigureAwait(false) as ICanFetchEventTypesFromPartitionedStream;
    }

    async Task<StreamFetcher<MongoDB.Events.Event>> CreateStreamFetcherForEventLog(ScopeId scopeId, CancellationToken cancellationToken) =>
        new(
            StreamId.EventLog,
            scopeId,
            await _streams.GetEventLog(scopeId, cancellationToken).ConfigureAwait(false),
            Builders<MongoDB.Events.Event>.Filter,
            _ => _.EventLogSequenceNumber,
            _ => _eventConverter.ToRuntimeStreamEvent(_),
            _ => _.Metadata.TypeId,
            _ => _.Metadata.TypeGeneration);

    StreamFetcher<MongoDB.Events.StreamEvent> CreateStreamFetcherForStreamEventCollection(IMongoCollection<Events.StreamEvent> collection, StreamId streamId, ScopeId scopeId, bool partitioned) =>
        new(
            streamId,
            scopeId,
            collection,
            Builders<Events.StreamEvent>.Filter,
            _ => _.StreamPosition,
            _ => _eventConverter.ToRuntimeStreamEvent(_, streamId, partitioned),
            _ => _.Metadata.TypeId,
            _ => _.Metadata.TypeGeneration,
            _ => _.Partition);
}