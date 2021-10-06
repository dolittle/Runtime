// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// An implementation of <see cref="IPerformMigrations"/>.
    /// </summary>
    public class MigrationPerformer : IPerformMigrations
    {
        readonly ResourceConfigurationsByTenant _resources;
        
        public MigrationPerformer(ResourceConfigurationsByTenant resources)
        {
            _resources = resources;
        }
        
        /// <inheritdoc/>
        public Task<Try> PerformForTenant(ICanMigrateAnEventStore eventStoreMigration, TenantId tenant)
        {
            return Task.FromResult(Try.Succeeded());
        }

        /// <inheritdoc/>
        public Task<Try> PerformForAllTenants(ICanMigrateAnEventStore eventStoreMigration)
        {
            return Task.FromResult(Try.Succeeded());
        }
    }
}
