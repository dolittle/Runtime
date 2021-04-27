// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents a <see cref="IStreams" />.
    /// </summary>
    [SingletonPerTenant]
    public class Streams : EventStoreConnection, IStreams
    {
        const string EventLogCollectionName = "event-log";
        const string StreamDefinitionCollectionName = "stream-definitions";

        readonly ILogger _logger;
        readonly IMongoCollection<StreamDefinition> _streamDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Streams"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Streams(DatabaseConnection connection, ILogger logger)
            : base(connection)
        {
            _logger = logger;

            DefaultEventLog = Database.GetCollection<Events.Event>(EventLogCollectionName);
            _streamDefinitions = Database.GetCollection<StreamDefinition>(StreamDefinitionCollectionName);

            CreateCollectionsAndIndexesForEventLog();
            CreateCollectionsAndIndexesForStreamDefinitions();
        }

        /// <inheritdoc/>
        public IMongoCollection<Events.Event> DefaultEventLog { get; }

        /// <inheritdoc/>
        public Task<IMongoCollection<Events.StreamEvent>> Get(ScopeId scopeId, StreamId streamId, CancellationToken token)
        {
            if (streamId == StreamId.EventLog) throw new CannotGetEventLogStream();
            return GetStreamCollection(scopeId, streamId, token);
        }

        /// <inheritdoc/>
        public Task<IMongoCollection<Events.StreamEvent>> GetPublic(StreamId streamId, CancellationToken token) =>
            GetPublicStreamCollection(streamId, token);

        /// <inheritdoc/>
        public Task<IMongoCollection<StreamDefinition>> GetDefinitions(ScopeId scopeId, CancellationToken token) =>
            scopeId == ScopeId.Default ? Task.FromResult(_streamDefinitions) : GetScopedStreamDefinitions(scopeId, token);

        /// <inheritdoc/>
        public Task<IMongoCollection<Events.Event>> GetEventLog(ScopeId scopeId, CancellationToken token) =>
            scopeId == ScopeId.Default ? Task.FromResult(DefaultEventLog) : GetScopedEventLog(scopeId, token);

        static string CollectionNameForStream(StreamId streamId) => $"stream-{streamId.Value}";

        static string CollectionNameForPublicStream(StreamId streamId) => $"public-stream-{streamId.Value}";

        static string CollectionNameForScopedEventLog(ScopeId scope) => $"x-{scope.Value}-{EventLogCollectionName}";

        static string CollectionNameForScopedStreamDefinitions(ScopeId scope) => $"x-{scope.Value}-{StreamDefinitionCollectionName}";

        static string CollectionNameForScopedStream(ScopeId scope, StreamId stream) => $"x-{scope.Value}-stream-{stream.Value}";

        Task<IMongoCollection<Events.StreamEvent>> GetStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken) =>
            scope == ScopeId.Default ? GetStreamCollection(stream, cancellationToken) : GetScopedStreamCollection(scope, stream, cancellationToken);

        async Task<IMongoCollection<Events.StreamEvent>> GetStreamCollection(StreamId stream, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForStream(stream));
            await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<Events.StreamEvent>> GetScopedStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForScopedStream(scope, stream));
            await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<StreamDefinition>> GetScopedStreamDefinitions(ScopeId scope, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<StreamDefinition>(CollectionNameForScopedStreamDefinitions(scope));
            await CreateCollectionsIndexesForStreamDefinitionsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<Events.Event>> GetScopedEventLog(ScopeId scope, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<Events.Event>(CollectionNameForScopedEventLog(scope));
            await CreateCollectionsIndexesForEventLogAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        async Task<IMongoCollection<Events.StreamEvent>> GetPublicStreamCollection(StreamId stream, CancellationToken cancellationToken)
        {
            var collection = Database.GetCollection<Events.StreamEvent>(CollectionNameForPublicStream(stream));
            await CreateCollectionsAndIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        void CreateCollectionsAndIndexesForEventLog()
        {
            _logger.CreatingIndexesFor(EventLogCollectionName);
            DefaultEventLog.Indexes.CreateOne(new CreateIndexModel<Events.Event>(
                Builders<Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)));

            DefaultEventLog.Indexes.CreateOne(new CreateIndexModel<Events.Event>(
                Builders<Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)));
        }

        void CreateCollectionsAndIndexesForStreamDefinitions()
        {
            _logger.CreatingIndexesFor(StreamDefinitionCollectionName);
            _streamDefinitions.Indexes.CreateOne(new CreateIndexModel<StreamDefinition>(
                Builders<StreamDefinition>.IndexKeys
                    .Ascending(_ => _.StreamId)));
        }

        async Task CreateCollectionsIndexesForEventLogAsync(IMongoCollection<Events.Event> eventLog, CancellationToken cancellationToken)
        {
            _logger.CreatingIndexesFor(eventLog.CollectionNamespace.CollectionName);
            await eventLog.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.Event>(
                Builders<Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await eventLog.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.Event>(
                Builders<Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task CreateCollectionsAndIndexesForStreamEventsAsync(IMongoCollection<Events.StreamEvent> stream, CancellationToken cancellationToken)
        {
            _logger.CreatingIndexesFor(stream.CollectionNamespace.CollectionName);
            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.StreamEvent>(
                    Builders<Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventLogSequenceNumber),
                    new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.StreamEvent>(
                    Builders<Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.StreamEvent>(
                    Builders<Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Partition)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Events.StreamEvent>(
                    Builders<Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)
                        .Ascending(_ => _.Aggregate.TypeId)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task CreateCollectionsIndexesForStreamDefinitionsAsync(IMongoCollection<StreamDefinition> streamDefinitions, CancellationToken cancellationToken)
        {
            _logger.CreatingIndexesFor(streamDefinitions.CollectionNamespace.CollectionName);
            await streamDefinitions.Indexes.CreateOneAsync(
                new CreateIndexModel<StreamDefinition>(
                    Builders<StreamDefinition>.IndexKeys
                        .Ascending(_ => _.StreamId)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
