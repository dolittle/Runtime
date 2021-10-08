// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings.Definition;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Version = Dolittle.Runtime.Versioning.Version;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateAnEventStore" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class Migrator : ICanMigrateAnEventStore
    {
        static readonly Version _from;
        static readonly Version _to;
        readonly IEventStoreConnections _eventStoreConnections;
        readonly IEmbeddingStoreConnections _embeddingStoreConnections;
        readonly IMongoCollectionMigrator _collectionMigrator;
        readonly IPerformMigrationStepsInOrder _migrationStepsPerformer;
        readonly IConvertOldEventSourceId _oldEventSourceIdConverter;
        readonly ILogger _logger;

        static Migrator()
        {
            _from = Version.NotSet with
            {
                Major = 5
            };
            _to = Version.NotSet with
            {
                Major = 7
            };
        }
        public Migrator(
            IEventStoreConnections eventStoreConnections,
            IEmbeddingStoreConnections embeddingStoreConnections,
            IMongoCollectionMigrator collectionMigrator,
            IPerformMigrationStepsInOrder migrationStepsPerformer,
            IConvertOldEventSourceId oldEventSourceIdConverter,
            ILogger<Migrator> logger)
        {
            _eventStoreConnections = eventStoreConnections;
            _embeddingStoreConnections = embeddingStoreConnections;
            _collectionMigrator = collectionMigrator;
            _migrationStepsPerformer = migrationStepsPerformer;
            _oldEventSourceIdConverter = oldEventSourceIdConverter;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Try> Migrate(EventStoreConfiguration eventStoreConfiguration)
        {
            try
            {
                _logger.LogInformation("Migrating MongoDB event store to version {Version}", _to);
                var eventStoreConnection = _eventStoreConnections.GetFor(eventStoreConfiguration);
                var collectionNames = await (await eventStoreConnection.Database.ListCollectionNamesAsync().ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
                return await _migrationStepsPerformer.Perform(
                    eventStoreConnection.Database,
                    (session, cancellationToken) => CreateEventStoreMigrationSteps(eventStoreConnection.Database, session, collectionNames.ToArray(), cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        /// <inheritdoc />
        public async Task<Try> Migrate(EventStoreConfiguration eventStoreConfiguration, EmbeddingsConfiguration embeddingsConfiguration)
        {
            try
            {
                var eventStoreConnection = _eventStoreConnections.GetFor(eventStoreConfiguration);
                var embeddingStoreConnection = _embeddingStoreConnections.GetFor(embeddingsConfiguration);
                
                var eventStoreCollectionNames = await (await eventStoreConnection.Database.ListCollectionNamesAsync().ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
                
                var embeddings = await GetEmbeddings(embeddingStoreConnection.Database).ConfigureAwait(false);
                
                return await _migrationStepsPerformer.Perform(
                    eventStoreConnection.Database,
                    (session, cancellationToken) =>
                    {
                        var steps = CreateEventStoreMigrationSteps(
                            eventStoreConnection.Database,
                            session,
                            eventStoreCollectionNames.ToArray(),
                            cancellationToken).ToList();
                        steps.AddRange(CreateEmbeddingMigrationScripts(
                            embeddings,    
                            eventStoreConnection.Database,
                            session,
                            eventStoreCollectionNames.ToArray(),
                            cancellationToken));
                        return steps;
                    });
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        IEnumerable<Task> CreateEmbeddingMigrationScripts(IDictionary<EmbeddingId, IEnumerable<ProjectionKey>> embeddings, IMongoDatabase database, IClientSessionHandle session, string[] collectionNames, CancellationToken cancellationToken)
        {
            var steps = new List<Task>();

            foreach (var (embedding, keys) in embeddings)
            {
                steps.Add(_collectionMigrator.Migrate(database, session, "aggregates", new EmbeddingAggregatesConverter(_oldEventSourceIdConverter, embedding, keys), cancellationToken));
                steps.Add(_collectionMigrator.Migrate(database, session, "event-log", new EmbeddingEventLogConverter(_oldEventSourceIdConverter, embedding, keys), cancellationToken));
                steps.Add(_collectionMigrator.Migrate(database, session, GetEventStreams(collectionNames).Where(IsNonScopedEventStream), new EmbeddingStreamConverter(_oldEventSourceIdConverter, embedding, keys), cancellationToken));
            }
            return steps;
        }
        static async Task<IDictionary<EmbeddingId, IEnumerable<ProjectionKey>>> GetEmbeddings(IMongoDatabase embeddingStore)
        {
            var embeddings = new Dictionary<EmbeddingId, IEnumerable<ProjectionKey>>();

            var embeddingStateCollectionNames = await (await embeddingStore.ListCollectionNamesAsync().ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
            foreach (var embeddingStateCollectionName in embeddingStateCollectionNames)
            {
                var keys = await embeddingStore
                    .GetCollection<Old.Embeddings.State.Embedding>(embeddingStateCollectionName)
                    .Find(Builders<Old.Embeddings.State.Embedding>.Filter.Empty)
                    .Project(_ => _.Key)
                    .ToListAsync()
                    .ConfigureAwait(false);
                embeddings.Add(Guid.Parse(embeddingStateCollectionName.Replace("embedding-", "")), keys.Select(_ => new ProjectionKey(_)));
            }
            
            return embeddings;
        }
        
        IEnumerable<Task> CreateEventStoreMigrationSteps(IMongoDatabase database, IClientSessionHandle session, string[] collectionNames, CancellationToken cancellationToken)
            => new []
            {
                MigrateAggregates(database, session, cancellationToken),
                MigrateEventLogs(database, session, collectionNames, cancellationToken),
                MigrateStreams(database, session, collectionNames, cancellationToken),
                MigrateSubscriptionStates(database, session, collectionNames, cancellationToken)
            };

        Task MigrateSubscriptionStates(IMongoDatabase database, IClientSessionHandle session, IEnumerable<string> collectionNames, CancellationToken cancellationToken)
            => _collectionMigrator.Migrate(database, session, GetSubscriptionStates(collectionNames), new SubscriptionStatesConverter(), cancellationToken);
        Task MigrateStreams(IMongoDatabase database, IClientSessionHandle session, IEnumerable<string> collectionNames, CancellationToken cancellationToken)
            => _collectionMigrator.Migrate(database, session, GetEventStreams(collectionNames), new StreamConverter(), cancellationToken);
        Task MigrateEventLogs(IMongoDatabase database, IClientSessionHandle session, IEnumerable<string> collectionNames, CancellationToken cancellationToken)
            => _collectionMigrator.Migrate(database, session, GetEventLogs(collectionNames), new EventLogConverter(), cancellationToken);

        Task MigrateAggregates(IMongoDatabase database, IClientSessionHandle session, CancellationToken cancellationToken)
            => _collectionMigrator.Migrate(database, session, "aggregates", new AggregatesConverter(), cancellationToken);

        static bool IsScopedEventStream(string collectionName)
            => collectionName.Contains("x-") && IsEventStreamCollection(collectionName);
        static bool IsNonScopedEventStream(string collectionName)
            => !collectionName.Contains("x-") && IsEventStreamCollection(collectionName);
        static bool IsEventStreamCollection(string collectionName)
            => collectionName.Contains("stream-")
                && !collectionName.Contains("stream-definitions")
                && !collectionName.Contains("stream-processor-states");

        static IEnumerable<string> GetSubscriptionStates(IEnumerable<string> collectionNames)
            => collectionNames.Where(_ => _.Contains("subscription-states"));
        static IEnumerable<string> GetEventStreams(IEnumerable<string> collectionNames)
            => collectionNames.Where(_ => IsScopedEventStream(_) || IsNonScopedEventStream(_));
        static IEnumerable<string> GetEventLogs(IEnumerable<string> collectionNames)
            => collectionNames.Where(_ => _.Contains("event-log"));
    }
}
