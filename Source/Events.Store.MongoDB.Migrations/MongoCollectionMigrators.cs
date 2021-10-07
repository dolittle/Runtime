// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMongoCollectionMigrators"/>.
    /// </summary>
    public class MongoCollectionMigrators : IMongoCollectionMigrators
    {
        /// <inheritdoc />
        public IMongoCollectionMigrator Create(IClientSessionHandle session, DatabaseConnection connection)
            => new MongoCollectionMigrator(session, connection);
    }
}