// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Defines a system that knows about <see cref="IMongoCollectionMigrator"/>.
    /// </summary>
    public interface IMongoCollectionMigrators
    {
        IMongoCollectionMigrator Create(IClientSessionHandle session, DatabaseConnection connection);
    }
}