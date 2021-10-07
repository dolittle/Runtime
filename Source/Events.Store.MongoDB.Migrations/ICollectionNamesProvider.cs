// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public interface ICollectionNamesProvider
    {
        Task<ICollectionNames> Provide(DatabaseConnection connection, IClientSessionHandle session, CancellationToken cancellationToken);
    }
}