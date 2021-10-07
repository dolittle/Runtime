// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMongoCollectionMigrator"/>.
    /// </summary>
    public class MongoCollectionMigrator : IMongoCollectionMigrator
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
            try
            {
                var collection = _connection.Database.GetCollection<TOld>(collectionName);
                var bsonCollection = _connection.Database.GetCollection<TNew>(collectionName);
                var oldDocuments = await collection.Find(_ => true).ToListAsync(cancellationToken);
                if (!oldDocuments.Any())
                {
                    return;
                }
                await bsonCollection.DeleteManyAsync(Builders<TNew>.Filter.Empty, cancellationToken).ConfigureAwait(false);
                await bsonCollection.InsertManyAsync(oldDocuments.Select(converter), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new FailedMigratingCollection(collectionName, ex);
            }
        }
    }
}