// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public interface ICanMigrate
    {
        Task<Try> Migrate(DatabaseConnection connection);
    }
}