// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters;
using Dolittle.Runtime.Events.Streams;
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

            EventLog = connection.Database.GetCollection<Event>(Constants.EventLogCollection);
            PublicEvents = connection.Database.GetCollection<PublicEvent>(Constants.PublicEventsCollection);
            Aggregates = connection.Database.GetCollection<AggregateRoot>(Constants.AggregateRootInstanceCollection);
            StreamProcessorStates = connection.Database.GetCollection<StreamProcessorState>(Constants.StreamProcessorStateCollection);
            TypePartitionFilterDefinitions = connection.Database.GetCollection<TypePartitionFilterDefinition>(Constants.TypePartitionFilterDefinitionCollection);

            CreateCollectionsAndIndexes();
        }

        /// <summary>
        /// Gets the <see cref="IMongoClient"/> configured for the MongoDB database.
        /// </summary>
        public IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{Event}"/> where Events in the event log are stored.
        /// </summary>
        public IMongoCollection<Event> EventLog { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{PublicEvent}" /> where public Events are stored.
        /// </summary>
        public IMongoCollection<PublicEvent> PublicEvents { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{AggregateRoot}"/> where Aggregate Roots are stored.
        /// </summary>
        public IMongoCollection<AggregateRoot> Aggregates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamProcessorState}" /> where <see cref="StreamProcessorState" >stream processor states</see> are stored.
        /// </summary>
        public IMongoCollection<StreamProcessorState> StreamProcessorStates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{TypePartitionFilterDefinition}" /> where <see cref="TypePartitionFilterDefinition" >type partition filter definitions</see> are stored.
        /// </summary>
        public IMongoCollection<TypePartitionFilterDefinition> TypePartitionFilterDefinitions { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamEvent}" /> that represents a stream of events.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<MongoDB.Events.StreamEvent>> GetStreamCollectionAsync(StreamId stream, CancellationToken cancellationToken = default)
        {
            var collection = _connection.Database.GetCollection<MongoDB.Events.StreamEvent>(Constants.CollectionNameForStream(stream));
            await CreateCollectionsAndIndexesForStreamAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{ReceivedEvent}" /> that represents a collection of the events received from a microservice.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{StreamEvent}" />.</returns>
        public async Task<IMongoCollection<ReceivedEvent>> GetReceivedEventsCollectionAsync(Microservice microservice, CancellationToken cancellationToken = default)
        {
            var collection = _connection.Database.GetCollection<ReceivedEvent>(Constants.CollectionNameForReceivedEvents(microservice));
            await CreateCollectionsAndIndexesForReceivedEventsAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        void CreateCollectionsAndIndexes()
        {
            CreateCollectionsAndIndexesForEventLog();
            CretaeCollectionsAndIndexesForPublicEvents();
            CreateCollectionsAndIndexesForAggregates();
            CreateCollectionsAndIndexesForStreamProcessorStates();
            CreateCollectionsAndIndexesForTypePartitionFilterDefinitions();
        }

        void CreateCollectionsAndIndexesForEventLog()
        {
            EventLog.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)));

            EventLog.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)));
        }

        void CretaeCollectionsAndIndexesForPublicEvents()
        {
            PublicEvents.Indexes.CreateOne(new CreateIndexModel<PublicEvent>(
                Builders<PublicEvent>.IndexKeys
                    .Ascending(_ => _.Metadata.EventLogVersion),
                new CreateIndexOptions { Unique = true }));
        }

        async Task CreateCollectionsAndIndexesForStreamAsync(IMongoCollection<Events.StreamEvent> stream, CancellationToken cancellationToken = default)
        {
            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoDB.Events.StreamEvent>(
                    Builders<MongoDB.Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventLogVersion),
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
                        .Ascending(_ => _.Metadata.Partition)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<MongoDB.Events.StreamEvent>(
                    Builders<MongoDB.Events.StreamEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)
                        .Ascending(_ => _.Aggregate.TypeId)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task CreateCollectionsAndIndexesForReceivedEventsAsync(IMongoCollection<ReceivedEvent> stream, CancellationToken cancellationToken = default)
        {
            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<ReceivedEvent>(
                    Builders<ReceivedEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.OriginEventLogVersion)
                        .Ascending(_ => _.Metadata.Microservice)
                        .Ascending(_ => _.Metadata.ProducerTenant),
                    new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<ReceivedEvent>(
                    Builders<ReceivedEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<ReceivedEvent>(
                    Builders<ReceivedEvent>.IndexKeys
                        .Ascending(_ => _.Metadata.ProducerTenant)
                        .Ascending(_ => _.Metadata.Microservice)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        void CreateCollectionsAndIndexesForAggregates()
        {
            Aggregates.Indexes.CreateOne(new CreateIndexModel<AggregateRoot>(
                Builders<AggregateRoot>.IndexKeys
                    .Ascending(_ => _.EventSource)
                    .Ascending(_ => _.AggregateType),
                new CreateIndexOptions { Unique = true }));
        }

        void CreateCollectionsAndIndexesForStreamProcessorStates()
        {
            StreamProcessorStates.Indexes.CreateOne(new CreateIndexModel<StreamProcessorState>(
                Builders<StreamProcessorState>.IndexKeys
                    .Ascending(_ => _.Id)));
        }

        void CreateCollectionsAndIndexesForTypePartitionFilterDefinitions()
        {
            TypePartitionFilterDefinitions.Indexes.CreateOne(new CreateIndexModel<TypePartitionFilterDefinition>(
                Builders<TypePartitionFilterDefinition>.IndexKeys
                    .Ascending(_ => _.Id)));
        }
    }
}