// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="MigrateAggregates"/>.
    /// </summary>
    public class MigrateAggregates
    {
        readonly IMongoCollectionMigrators _migrators;
        
        public MigrateAggregates(IMongoCollectionMigrators migrators)
        {
            _migrators = migrators;
        }

        public Task Migrate(IClientSessionHandle session, DatabaseConnection connection, CancellationToken cancellationToken)
        {
            var migrator = _migrators.Create(session, connection);
            return migrator.MigrateCollection<Models.Aggregates.AggregateRoot, Aggregates.AggregateRoot>(
                "aggregates",
                old => new(old.EventSource.ToString(), old.AggregateType, old.Version),
                cancellationToken);
        }
    }
}