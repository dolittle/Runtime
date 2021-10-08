// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Defines a system that can migrate a <see cref="IMongoCollection{TDocument}" />
    /// </summary>
    public interface IMongoCollectionMigrator
    {
        Task Migrate<TOld, TNew>(
            IMongoDatabase database,
            IClientSessionHandle session,
            string collectionName,
            IConvertFromOldToNew<TOld, TNew> converter,
            CancellationToken cancellationToken);
        Task Migrate<TOld, TNew>(
            IMongoDatabase database,
            IClientSessionHandle session,
            IEnumerable<string> collectionNames,
            IConvertFromOldToNew<TOld, TNew> converter,
            CancellationToken cancellationToken);
    }
}