// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Represents a connection to the MongoDB EventStore database.
    /// </summary>
    [SingletonPerTenant]
    public class EventStoreConnection
    {
        readonly DatabaseConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreConnection"/> class.
        /// </summary>
        /// <param name="connection">A connection to the MongoDB database.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventStoreConnection(DatabaseConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;

            MongoClient = connection.MongoClient;

            EventLog = connection.Database.GetCollection<MongoDB.Events.Event>(Constants.EventLogCollection);
            Aggregates = connection.Database.GetCollection<AggregateRoot>(Constants.AggregateRootInstanceCollection);

            StreamProcessorStates = connection.Database.GetCollection<StreamProcessorState>(Constants.StreamProcessorStateCollection);
            TypePartitionFilterDefinitions = connection.Database.GetCollection<TypePartitionFilterDefinition>(Constants.TypePartitionFilterDefinitionCollection);

            CreateIndexes();
        }

        /// <summary>
        /// Gets the <see cref="IMongoClient"/> configured for the MongoDB database.
        /// </summary>
        public IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{Event}"/> where Events in the event log are stored.
        /// </summary>
        public IMongoCollection<MongoDB.Events.Event> EventLog { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{AggregateRoot}"/> where Aggregate Roots are stored.
        /// </summary>
        public IMongoCollection<AggregateRoot> Aggregates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamProcessorState}" /> where <see cref="StreamProcessorState" >stream processor states</see> are stored.
        /// </summary>
        public IMongoCollection<StreamProcessorState> StreamProcessorStates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{TDocument}" /> for <see cref="TypePartitionFilterDefinition" />.
        /// </summary>
        public IMongoCollection<TypePartitionFilterDefinition> TypePartitionFilterDefinitions { get; }

        /// <summary>
        /// Gets the correct <see cref="IMongoCollection{TDocument}" /> for <see cref="Events.StreamEvent" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The collection.</returns>
        public Task<IMongoCollection<Events.StreamEvent>> GetStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken) =>
            scope == ScopeId.Default ? GetStreamCollection(stream, cancellationToken) : GetScopedStreamCollection(scope, stream, cancellationToken);

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamEvent}" /> that represents a stream of events.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<Events.StreamEvent>> GetStreamCollection(StreamId stream, CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<Events.StreamEvent>(Constants.CollectionNameForStream(stream));
            await CreateIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{ReceivedEvent}" /> that represents a collection of the events received from a microservice.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<Events.StreamEvent>> GetScopedStreamCollection(ScopeId scope, StreamId stream, CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<Events.StreamEvent>(Constants.CollectionNameForScopedStream(scope, stream));
            await CreateIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Gets the correct <see cref="IMongoCollection{TDocument}" /> for <see cref="StreamProcessorState" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The collection.</returns>
        public Task<IMongoCollection<StreamProcessorState>> GetStreamProcessorStateCollection(
            ScopeId scope,
            CancellationToken cancellationToken) =>
            scope == ScopeId.Default ? Task.FromResult(StreamProcessorStates)
                : GetScopedStreamProcessorStateCollection(scope, cancellationToken);

        /// <summary>
        /// Gets the scoped <see cref="IMongoCollection{T}" /> of <see cref="StreamProcessorState" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamProcessorState}" />.</returns>
        public async Task<IMongoCollection<StreamProcessorState>> GetScopedStreamProcessorStateCollection(
            ScopeId scope,
            CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<StreamProcessorState>(Constants.CollectionNameForScopedStreamProcessorStates(scope));
            await CreateIndexesForStreamProcessorStatesAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Gets the correct event log <see cref="IMongoCollection{TDocument}" /> for <see cref="Event" />.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The collection.</returns>
        public Task<IMongoCollection<MongoDB.Events.Event>> GetEventLogCollection(ScopeId scope, CancellationToken cancellationToken) =>
            scope == ScopeId.Default ? Task.FromResult(EventLog) : GetScopedEventLog(scope, cancellationToken);

        /// <summary>
        /// Gets the <see cref="IMongoCollection{ReceivedEvent}" /> that represents a collection of the events received from a microservice.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<MongoDB.Events.Event>> GetScopedEventLog(ScopeId scope, CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<MongoDB.Events.Event>(Constants.CollectionNameForScopedEventLog(scope));
            await CreateIndexesForEventLogAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamEvent}" /> that represents a stream of events.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<Events.StreamEvent>> GetPublicStreamCollection(StreamId stream, CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<Events.StreamEvent>(Constants.CollectionNameForPublicStream(stream));
            await CreateIndexesForStreamEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        void CreateIndexes()
        {
            CreateIndexesForEventLog();
            CreateIndexesForAggregates();
            CreateIndexesForStreamProcessorStates();
            CreateIndexesForTypePartitionFilterDefinitions();
        }

        void CreateIndexesForEventLog()
        {
            EventLog.Indexes.CreateOne(new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)));

            EventLog.Indexes.CreateOne(new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)));
        }

        void CreateIndexesForAggregates()
        {
            Aggregates.Indexes.CreateOne(new CreateIndexModel<AggregateRoot>(
                Builders<AggregateRoot>.IndexKeys
                    .Ascending(_ => _.EventSource)
                    .Ascending(_ => _.AggregateType),
                new CreateIndexOptions { Unique = true }));
        }

        /// <summary>
        /// Creates the compound index for <see cref="StreamProcessorState"/>.
        /// </summary>
        void CreateIndexesForStreamProcessorStates()
        {
            StreamProcessorStates.Indexes.CreateOne(
                new CreateIndexModel<StreamProcessorState>(
                Builders<StreamProcessorState>.IndexKeys
                    .Ascending(_ => _.ScopeId)
                    .Ascending(_ => _.EventProcessorId)
                    .Ascending(_ => _.SourceStreamId)));
        }

        void CreateIndexesForTypePartitionFilterDefinitions()
        {
            TypePartitionFilterDefinitions.Indexes.CreateOne(new CreateIndexModel<TypePartitionFilterDefinition>(
                Builders<TypePartitionFilterDefinition>.IndexKeys
                    .Ascending(_ => _.TargetStream)));
        }

        async Task CreateIndexesForStreamEventsAsync(IMongoCollection<Events.StreamEvent> stream, CancellationToken cancellationToken)
        {
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

        /// <summary>
        /// Creates the compound index for <see cref="StreamProcessorState"/>.
        /// </summary>
        /// <param name="streamProcessorState">Collection of <see cref="StreamProcessorState"/> to add indexes to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Task.</returns>
        async Task CreateIndexesForStreamProcessorStatesAsync(
            IMongoCollection<StreamProcessorState> streamProcessorState,
            CancellationToken cancellationToken)
        {
            await streamProcessorState.Indexes.CreateOneAsync(
                new CreateIndexModel<StreamProcessorState>(
                    Builders<StreamProcessorState>.IndexKeys
                        .Ascending(_ => _.ScopeId)
                        .Ascending(_ => _.EventProcessorId)
                        .Ascending(_ => _.SourceStreamId),
                    new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task CreateIndexesForEventLogAsync(IMongoCollection<MongoDB.Events.Event> stream, CancellationToken cancellationToken)
        {
            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoDB.Events.Event>(
                Builders<MongoDB.Events.Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
