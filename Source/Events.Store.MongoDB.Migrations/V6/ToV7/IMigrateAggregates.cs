// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.V6.Models.Aggregates;
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.V6.ToV7
{
    /// <summary>
    /// Defines a system that can perform v6 to v7 migration on aggregates collection. 
    /// </summary>
    public interface IMigrateAggregates : ICanMigrate, IMongoCollectionMigrator<Models.Aggregates.AggregateRoot, MongoDB.Aggregates.AggregateRoot>
    {
    }
    /// <summary>
    /// Represents an implementation of <see cref="IMigrateAggregates"/>.
    /// </summary>
    class MigrateAggregates : IMigrateAggregates
    {
        public Task<Try> Migrate(DatabaseConnection connection)
            => throw new NotImplementedException();
        public Task<Try> MigrateCollection(string collectionName, Func<AggregateRoot, Aggregates.AggregateRoot> converter)
            => throw new NotImplementedException();
    }

}