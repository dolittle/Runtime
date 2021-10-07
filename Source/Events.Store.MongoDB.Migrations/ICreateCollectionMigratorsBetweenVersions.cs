// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Versioning;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public interface ICreateCollectionMigratorsBetweenVersions
    {
        IEnumerable<ICanMigrateCollectionBetweenVersions> Create(Version from, Version to, ICollectionNames collectionNames, IMongoCollectionMigrator migrator);
    }
}