// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.EventLog;
using Dolittle.Runtime.Events.Store.MongoDB.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
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
            Aggregates = connection.Database.GetCollection<AggregateRoot>(Constants.AggregateRootInstanceCollection);
            StreamProcessorStates = connection.Database.GetCollection<StreamProcessorState>(Constants.StreamProcessorStateCollection);
            StreamEvents = connection.Database.GetCollection<StreamEvent>(Constants.StreamEventCollection);

            CreateCollectionsAndIndexes();
        }

        /// <summary>
        /// Gets the <see cref="IMongoClient"/> configured for the MongoDB database.
        /// </summary>
        public IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{Event}"/> where Events are stored.
        /// </summary>
        public IMongoCollection<Event> EventLog { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{AggregateRoot}"/> where Aggregate Roots are stored.
        /// </summary>
        public IMongoCollection<AggregateRoot> Aggregates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamProcessorState}" /> where <see cref="StreamProcessorState" >stream processor states</see> are stored.
        /// </summary>
        public IMongoCollection<StreamProcessorState> StreamProcessorStates { get; }

        /// <summary>
        /// Gets the <see cref="IMongoCollection{StreamEvent}" /> where <see cref="StreamEvent" >stream events</see> are stored.
        /// </summary>
        public IMongoCollection<StreamEvent> StreamEvents { get; }

        void CreateCollectionsAndIndexes()
        {
            CreateCollectionsAndIndexesForEventLog();
            CreateCollectionsAndIndexesForStreamEvents();
            CreateCollectionsAndIndexesForAggregates();
            CreateCollectionsAndIndexesForStreamProcessorStates();
        }

        void CreateCollectionsAndIndexesForEventLog()
        {
            EventLog.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Aggregate.EventSourceId)));

            EventLog.Indexes.CreateOne(new CreateIndexModel<Event>(
                Builders<Event>.IndexKeys
                    .Ascending(_ => _.Aggregate.EventSourceId)
                    .Ascending(_ => _.Aggregate.TypeId)));
        }

        void CreateCollectionsAndIndexesForStreamEvents()
        {
            StreamEvents.Indexes.CreateOne(new CreateIndexModel<StreamEvent>(
                Builders<StreamEvent>.IndexKeys
                    .Ascending(_ => _.StreamIdAndPosition)));

            StreamEvents.Indexes.CreateOne(new CreateIndexModel<StreamEvent>(
                Builders<StreamEvent>.IndexKeys
                    .Ascending(_ => _.StreamIdAndPosition.StreamId)));

            StreamEvents.Indexes.CreateOne(new CreateIndexModel<StreamEvent>(
                Builders<StreamEvent>.IndexKeys
                    .Ascending(_ => _.StreamIdAndPosition)
                    .Ascending(_ => _.PartitionId)));
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