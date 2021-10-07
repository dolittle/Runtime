// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public abstract class BaseMigrator<TOld, TNew> : ICanMigrateCollectionBetweenVersions
    {
        readonly ICollectionNames _collectionNames;
        readonly IMongoCollectionMigrator _migrator;
        
        protected BaseMigrator(ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
        {
            _collectionNames = collectionNames;
            _migrator = migrator;
        }
        public Task Migrate(IClientSessionHandle session, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            tasks.AddRange(GetCollections(_collectionNames)
                .Select(collectionName =>_migrator.MigrateCollection<TOld, TNew>(
                    collectionName,
                    Convert,
                    cancellationToken)));
            return Task.WhenAll(tasks);
        }

        protected abstract IEnumerable<string> GetCollections(ICollectionNames collectionNames);
        protected abstract TNew Convert(TOld old);
    }
}