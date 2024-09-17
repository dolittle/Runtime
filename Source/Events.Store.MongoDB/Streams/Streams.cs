// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents a <see cref="IStreams" />.
/// </summary>
[Singleton, PerTenant]
public class Streams : EventStoreConnection, IStreams
{
    public const string EventLogCollectionName = "event-log";
    const string StreamDefinitionCollectionName = "stream-definitions";

    readonly ILogger _logger;
    readonly IMongoCollection<MongoDB.Streams.StreamDefinition> _streamDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="Streams"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="IDatabaseConnection" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public Streams(IDatabaseConnection connection, ILogger logger)
        : base(connection)
    {
        _logger = logger;

        DefaultEventLog = Database.GetCollection<MongoDB.Events.Event>(EventLogCollectionName);
        _streamDefinitions = Database.GetCollection<MongoDB.Streams.StreamDefinition>(StreamDefinitionCollectionName);

        CreateCollectionsAndIndexesForEventLog();
        CreateCollectionsAndIndexesForStreamDefinitions();
    }

    /// <inheritdoc/>
    public IMongoCollection<MongoDB.Events.Event> DefaultEventLog { get; }
    public IMongoCollection<MongoDB.Events.StreamMetadata> EventLogMetadata { get; }

    /// <inheritdoc/>
    public Task<IMongoCollection<MongoDB.Events.StreamEvent>> Get(ScopeId scopeId, StreamId streamId, CancellationToken token)
    {
        if (streamId == StreamId.EventLog)
        {
            throw new CannotGetEventLogStream();
        }

        return GetStreamCollection(scopeId, streamId, token);
    }

    /// <inheritdoc/>
    public Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetPublic(StreamId streamId, CancellationToken token) =>
        GetPublicStreamCollection(streamId, token);

    /// <inheritdoc/>
    public Task<IMongoCollection<MongoDB.Streams.StreamDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token) =>
        scopeId == ScopeId.Default ? Task.FromResult(_streamDefinitions) : GetScopedStreamDefinitions(scopeId, token);

    /// <inheritdoc/>
    public Task<IMongoCollection<MongoDB.Events.Event>> GetEventLog(ScopeId scopeId, CancellationToken token) =>
        scopeId == ScopeId.Default ? Task.FromResult(DefaultEventLog) : GetScopedEventLog(scopeId, token);

    static string CollectionNameForStream(StreamId streamId) => $"stream-{streamId.Value}";

    static string CollectionNameForPublicStream(StreamId streamId) => $"public-stream-{streamId.Value}";

    static string CollectionNameForScopedEventLog(ScopeId scope) => $"x-{scope.Value}-{EventLogCollectionName}";

    static string CollectionNameForScopedStreamDefinitions(ScopeId scope) => $"x-{scope.Value}-{StreamDefinitionCollectionName}";

    static string CollectionNameForScopedStream(ScopeId scope, StreamId stream) => $"x-{scope.Value}-stream-{stream.Value}";

    Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken) =>
        scope == ScopeId.Default ? GetStreamCollection(stream, cancellationToken) : GetScopedStreamCollection(scope, stream, cancellationToken);

    async Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetStreamCollection(StreamId stream, CancellationToken cancellationToken)
    {
        var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForStream(stream));
        await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
        return collection;
    }

    async Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetScopedStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken)
    {
        var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForScopedStream(scope, stream));
        await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
        return collection;
    }

    async Task<IMongoCollection<MongoDB.Streams.StreamDefinition>> GetScopedStreamDefinitions(ScopeId scope, CancellationToken cancellationToken)
    {
        var collection = Database.GetCollection<MongoDB.Streams.StreamDefinition>(CollectionNameForScopedStreamDefinitions(scope));
        await CreateCollectionsIndexesForStreamDefinitionsAsync(collection, cancellationToken).ConfigureAwait(false);
        return collection;
    }

    async Task<IMongoCollection<MongoDB.Events.Event>> GetScopedEventLog(ScopeId scope, CancellationToken cancellationToken)
    {
        var collection = Database.GetCollection<MongoDB.Events.Event>(CollectionNameForScopedEventLog(scope));
        await CreateCollectionsIndexesForEventLogAsync(collection, cancellationToken).ConfigureAwait(false);
        return collection;
    }

    async Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetPublicStreamCollection(StreamId stream, CancellationToken cancellationToken)
    {
        var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForPublicStream(stream));
        await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
        return collection;
    }

    void CreateCollectionsAndIndexesForEventLog()
    {
        const string metadataEventsourceIndex = "Metadata.EventSource_1";
        const string oldAggregateIndex = "Metadata.EventSource_1_Aggregate.TypeId_1";
        const string aggregateIndex = "aggregate_version_index";

        var existing = DefaultEventLog.Indexes.List().ToList();
        var createAggregateVersionIndex = true;
        var createEventSourceIndex = true;
        foreach (var doc in existing)
        {
            if (doc.TryGetValue("name", out var nameValue))
            {
                var name = nameValue.AsString;
                switch (name)
                {
                    case aggregateIndex:
                        createAggregateVersionIndex = false;
                        break;
                    case metadataEventsourceIndex:
                        createEventSourceIndex = false;
                        break;
                    case oldAggregateIndex:
                        DefaultEventLog.Indexes.DropOne(oldAggregateIndex);
                        break;
                }
            }
        }

        Log.CreatingIndexesFor(_logger, EventLogCollectionName);
        if (createEventSourceIndex)
        {
            DefaultEventLog.Indexes.CreateOne(new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource), new CreateIndexOptions
                {
                    Unique = false,
                    Name = metadataEventsourceIndex
                }));
        }

        if (createAggregateVersionIndex)
        {
            DefaultEventLog.Indexes.CreateOne(new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Aggregate.TypeId)
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.Version),
                new CreateIndexOptions<MongoDB.Events.Event>
                {
                    Name = aggregateIndex,
                    Unique = true,
                    PartialFilterExpression = Builders<MongoDB.Events.Event>.Filter.Eq(_ => _.Aggregate.WasAppliedByAggregate, true)
                }
            ));
        }
    }

    void CreateCollectionsAndIndexesForStreamDefinitions()
    {
        Log.CreatingIndexesFor(_logger, StreamDefinitionCollectionName);
        _streamDefinitions.Indexes.CreateOne(new CreateIndexModel<MongoDB.Streams.StreamDefinition>(
            Builders<MongoDB.Streams.StreamDefinition>.IndexKeys
                .Ascending(_ => _.StreamId)));
    }

    async Task CreateCollectionsIndexesForEventLogAsync(IMongoCollection<MongoDB.Events.Event> eventLog, CancellationToken cancellationToken)
    {
        Log.CreatingIndexesFor(_logger, eventLog.CollectionNamespace.CollectionName);
        await eventLog.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await eventLog.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    async Task CreateCollectionsAndIndexesForStreamEventsAsync(IMongoCollection<Events.StreamEvent> stream, CancellationToken cancellationToken)
    {
        Log.CreatingIndexesFor(_logger, stream.CollectionNamespace.CollectionName);
        await stream.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.StreamEvent>(
                Builders<MongoDB.Events.StreamEvent>.IndexKeys
                    .Ascending(_ => _.Metadata.EventLogSequenceNumber),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await stream.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.StreamEvent>(
                Builders<MongoDB.Events.StreamEvent>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await stream.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.StreamEvent>(
                Builders<MongoDB.Events.StreamEvent>.IndexKeys
                    .Ascending(_ => _.Partition)),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await stream.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Events.StreamEvent>(
                Builders<MongoDB.Events.StreamEvent>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    async Task CreateCollectionsIndexesForStreamDefinitionsAsync(IMongoCollection<MongoDB.Streams.StreamDefinition> streamDefinitions,
        CancellationToken cancellationToken)
    {
        Log.CreatingIndexesFor(_logger, streamDefinitions.CollectionNamespace.CollectionName);
        await streamDefinitions.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Streams.StreamDefinition>(
                Builders<MongoDB.Streams.StreamDefinition>.IndexKeys
                    .Ascending(_ => _.StreamId)),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
