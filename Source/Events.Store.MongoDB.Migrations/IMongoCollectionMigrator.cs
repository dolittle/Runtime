// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Defines a system that can migrate a <see cref="IMongoCollection{TDocument}" />
    /// </summary>
    /// <typeparam name="TOld"></typeparam>
    /// <typeparam name="TNew"></typeparam>
    public interface IMongoCollectionMigrator<TOld, TNew>
    {
        Task<Try> MigrateCollection(string collectionName, Func<TOld, TNew> converter);
    }
}