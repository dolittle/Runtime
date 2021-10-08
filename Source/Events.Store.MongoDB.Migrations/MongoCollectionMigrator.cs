// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMongoCollectionMigrator"/>.
    /// </summary>
    public class MongoCollectionMigrator : IMongoCollectionMigrator
    {
        readonly ILogger<MongoCollectionMigrator> _logger;
        
        public MongoCollectionMigrator(ILogger<MongoCollectionMigrator> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task Migrate<TOld, TNew>(
            IMongoDatabase database,
            IClientSessionHandle session,
            string collectionName,
            IConvertFromOldToNew<TOld, TNew> converter,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Migrating {Collection}", collectionName);
                var collection = database.GetCollection<TOld>(collectionName);
                var bsonCollection = database.GetCollection<TNew>(collectionName);
                var oldDocuments = await collection.Find(_ => true).ToListAsync(cancellationToken);
                if (!oldDocuments.Any())
                {
                    return;
                }
                await bsonCollection.DeleteManyAsync(Builders<TNew>.Filter.Empty, cancellationToken).ConfigureAwait(false);
                await bsonCollection.InsertManyAsync(oldDocuments.Select(converter.Convert), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new FailedMigratingCollection(collectionName, ex);
            }
        }
        public Task Migrate<TOld, TNew>(IMongoDatabase database, IClientSessionHandle session, IEnumerable<string> collectionNames, IConvertFromOldToNew<TOld, TNew> converter, CancellationToken cancellationToken)
            => Task.WhenAll(collectionNames.Select(_ => Migrate(database, session, _, converter, cancellationToken)));
    }
}