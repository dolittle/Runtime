// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Processing;
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

            AllStream = connection.Database.GetCollection<Event>(Constants.AllStreamCollection);
            Aggregates = connection.Database.GetCollection<AggregateRoot>(Constants.AggregateRootInstanceCollection);
            StreamProcessorStates = connection.Database.GetCollection<StreamProcessorState>(Constants.StreamProcessorStateCollection);

            CreateCollectionsAndIndexes();
        }

        /// <summary>
        /// Gets the <see cref="IMongoClient"/> configured for the MongoDB database.
        /// </summary>
        public IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{Event}"/> where Events are stored.
        /// </summary>
        public IMongoCollection<Event> AllStream { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{AggregateRoot}"/> where Aggregate Roots are stored.
        /// </summary>
        public IMongoCollection<AggregateRoot> Aggregates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamProcessorState}" /> where <see cref="StreamProcessorState" >stream processor states</see> are stored.
        /// </summary>
        public IMongoCollection<StreamProcessorState> StreamProcessorStates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{Event}" /> that represents a stream of events.
        /// </summary>
        /// <param name="streamId">The <see cref="Runtime.Events.Processing.StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IMongoCollection{Event}" />.</returns>
        public async Task<IMongoCollection<Event>> GetStreamCollectionAsync(Runtime.Events.Processing.StreamId streamId, CancellationToken cancellationToken = default)
        {
            var collection = _connection.Database.GetCollection<Event>(Constants.CollectionNameForStream(streamId));
            await CreateCollectionsAndIndexesForStreamAsync(collection, cancellationToken).ConfigureAwait(false);
            return collection;
        }

        void CreateCollectionsAndIndexes()
        {
            CreateCollectionsAndIndexesForStream(AllStream);
            CreateCollectionsAndIndexesForAggregates();
            CreateCollectionsAndIndexesForStreamProcessorStates();
        }

        void CreateCollectionsAndIndexesForStream(IMongoCollection<Event> stream)
        {
            stream.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)));

            stream.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Metadata.EventSource)
                    .Ascending(_ => _.Aggregate.TypeId)));
        }

        async Task CreateCollectionsAndIndexesForStreamAsync(IMongoCollection<Event> stream, CancellationToken cancellationToken = default)
        {
            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Event>(
                    Builders<Event>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await stream.Indexes.CreateOneAsync(
                new CreateIndexModel<Event>(
                    Builders<Event>.IndexKeys
                        .Ascending(_ => _.Metadata.EventSource)
                        .Ascending(_ => _.Aggregate.TypeId)),
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
    }
}