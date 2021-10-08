// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters;
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
        readonly IMongoCollectionMigrator _collectionMigrator;
        readonly IPerformMigrationSteps _migrationStepsPerformer;
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
            IMongoCollectionMigrator collectionMigrator,
            IPerformMigrationSteps migrationStepsPerformer,
            ILogger<Migrator> logger)
        {
            _eventStoreConnections = eventStoreConnections;
            _collectionMigrator = collectionMigrator;
            _logger = logger;
            _migrationStepsPerformer = migrationStepsPerformer;
        }

        /// <inheritdoc />
        public async Task<Try> Migrate(EventStoreConfiguration configuration)
        {
            try
            {
                var connection = _eventStoreConnections.GetFor(configuration);
                var collectionNames = await (await connection.Database.ListCollectionNamesAsync().ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
                return await _migrationStepsPerformer.Perform(
                    connection.Database,
                    (session, cancellationToken) => CreateMigrationSteps(connection.Database, session, collectionNames.ToArray(), cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        IEnumerable<Task> CreateMigrationSteps(IMongoDatabase database, IClientSessionHandle session, string[] collectionNames, CancellationToken cancellationToken)
            => new[]
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
