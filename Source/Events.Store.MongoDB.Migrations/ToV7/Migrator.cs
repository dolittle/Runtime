// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Version = Dolittle.Runtime.Versioning.Version;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateAnEventStore" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class Migrator : ICanMigrateAnEventStore
    {
        static readonly Version _from;
        static readonly Version _to;
        readonly IEventStoreConnections _eventStoreConnections;
        readonly IMongoCollectionMigrators _collectionMigrators;
        readonly ICreateCollectionMigratorsBetweenVersions _collectionMigratorsBetweenVersions;
        readonly ICollectionNamesProvider _collectionNamesProvider;
        readonly ILogger _logger;

        static Migrator()
        {
            _from = Version.NotSet with { Major = 5 };
            _to = Version.NotSet with { Major = 7 };
        }
        public Migrator(
            IEventStoreConnections eventStoreConnections,
            IMongoCollectionMigrators collectionMigrators,
            ICreateCollectionMigratorsBetweenVersions collectionMigratorsBetweenVersions,
            ICollectionNamesProvider collectionNamesProvider,
            ILogger<Migrator> logger)
        {
            _eventStoreConnections = eventStoreConnections;
            _collectionMigrators = collectionMigrators;
            _collectionMigratorsBetweenVersions = collectionMigratorsBetweenVersions;
            _collectionNamesProvider = collectionNamesProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Try> Migrate(EventStoreConfiguration configuration)
        {
            try
            {
                var connection = _eventStoreConnections.GetFor(configuration);
                using var session = await connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                using var cts = new CancellationTokenSource();
                return await TryDoAllMigrations(session, connection, cts).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        async Task<Try> TryDoAllMigrations(IClientSessionHandle session, DatabaseConnection connection, CancellationTokenSource cts)
        {
            try
            {
                var collectionMigrator = _collectionMigrators.Create(session, connection);
                session.StartTransaction();
                _logger.LogInformation("Start migrating");
                var watch = new Stopwatch();
                watch.Start();
                var tasks = _collectionMigratorsBetweenVersions
                    .Create(
                        _from,
                        _to,
                        await _collectionNamesProvider.Provide(connection, session, cts.Token).ConfigureAwait(false),
                        collectionMigrator)
                    .Select(_ => _.Migrate(session, cts.Token));
                await Task.WhenAll(tasks).ConfigureAwait(false);
                watch.Stop();
                _logger.LogInformation("Migration finished after {Time}", watch.Elapsed);
                return Try.Succeeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating");
                cts.Cancel();
                await session.AbortTransactionAsync().ConfigureAwait(false);
                return ex;
            }
            
        }
    }
}
