// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Defines a system that can perform migrations
    /// </summary>
    public interface IPerformMigrations
    {
        /// <summary>
        /// Tries to perform an event store migration for a specific tenant.
        /// </summary>
        /// <param name="eventStoreMigration">The event store migration to perform.</param>
        /// <param name="tenant">The tenant id to perform the migration for.</param>
        /// <returns>A <see cref="Task{T}"/> that when resolved returns a <see cref="Try"/> representing the result of the migration.</returns>
        Task<Try> PerformForTenant(ICanMigrateAnEventStore eventStoreMigration, TenantId tenant);

        /// <summary>
        /// Tries to perform an event storate migration for all tenants.
        /// </summary>
        /// <param name="eventStoreMigration">The event store migration to perform.</param>
        /// <returns>A <see cref="Task{T}"/> that when resolved returns a <see cref="Try"/> representing the result of the migration.</returns>
        Task<Try> PerformForAllTenants(ICanMigrateAnEventStore eventStoreMigration);
    }
}
