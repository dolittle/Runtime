// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateAnEventStore" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class Migrator : ICanMigrateAnEventStore
    {
        readonly IEventStoreConnections _eventStoreConnections;
        readonly MigrateAggregates _aggregatesMigrator;

        public Migrator(IEventStoreConnections eventStoreConnections, MigrateAggregates aggregatesMigrator)
        {
            _eventStoreConnections = eventStoreConnections;
            _aggregatesMigrator = aggregatesMigrator;
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
                var tasks = new[]
                {
                    _aggregatesMigrator.Migrate(session, connection, cts.Token)
                };
                await Task.WhenAll(tasks).ConfigureAwait(false);
                return Try.Succeeded();
            }
            catch (Exception ex)
            {
                cts.Cancel();
                await session.AbortTransactionAsync().ConfigureAwait(false);
                return ex;
            }
            
        }
    }
}
