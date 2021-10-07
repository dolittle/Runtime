// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMongoCollectionMigrator"/>.
    /// </summary>
    class MongoCollectionMigrator : IMongoCollectionMigrator
    {
        readonly DatabaseConnection _connection;
        readonly IClientSessionHandle _session;
        
        public MongoCollectionMigrator(IClientSessionHandle session, DatabaseConnection connection)
        {
            _session = session;
            _connection = connection;
        }
        /// <inheritdoc />
        public async Task MigrateCollection<TOld, TNew>(string collectionName, Func<TOld, TNew> converter, CancellationToken cancellationToken)
        {
            var collection = _connection.Database.GetCollection<TOld>(collectionName);
            var bsonCollection = _connection.Database.GetCollection<BsonDocument>(collectionName);
            var oldDocuments = await collection.Find(_session, _ => true).ToListAsync(cancellationToken);
            await bsonCollection.DeleteManyAsync(_session, Builders<BsonDocument>.Filter.Empty, cancellationToken: cancellationToken).ConfigureAwait(false);
            await bsonCollection.InsertManyAsync(_session, oldDocuments.Select(converter).Select(_ => _.ToBsonDocument()), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}