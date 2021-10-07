// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Versioning;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public class CreateCollectionMigratorsBetweenVersions : ICreateCollectionMigratorsBetweenVersions
    {
        public IEnumerable<ICanMigrateCollectionBetweenVersions> Create(Version from, Version to, ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
        {
            return new ICanMigrateCollectionBetweenVersions[]
            {
                new ToV7.AggregatesMigrator(collectionNames, migrator),
                new ToV7.EventLogMigrator(collectionNames, migrator),
                new ToV7.StreamMigrator(collectionNames, migrator),
                new ToV7.SubscriptionStatesMigrator(collectionNames, migrator)
            };
        }
    }
}