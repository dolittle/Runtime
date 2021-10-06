// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB;
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
            var configuration = GetConfiguration(tenant);
            if (!configuration.Success)
            {
                return Task.FromResult(Try.Failed(configuration.Exception));
            }
            return eventStoreMigration.Migrate(configuration.Result);
        }

            /// <inheritdoc/>
        public async Task<Try> PerformForAllTenants(ICanMigrateAnEventStore eventStoreMigration)
        {
            var configurations = GetAllConfigurations();
            if (!configurations.Success)
            {
                return configurations.Exception;
            }
            foreach (var configuration in configurations.Result)
            {
                var result = await eventStoreMigration.Migrate(configuration).ConfigureAwait(false);
                if (!result.Success)
                {
                    return result;
                }
            }
            return Try.Succeeded();
        }

        Try<EventStoreConfiguration> GetConfiguration(TenantId tenant)
        {
            try
            {
                if (!_resources.ContainsKey(tenant))
                {
                    return new TenantNotConfigured(tenant);
                }
                var resources = _resources[tenant];
                return !resources.ContainsKey("eventStore") 
                    ? new EventStoreNotConfiguredForTenant(tenant)
                    : Try<EventStoreConfiguration>.Succeeded(resources["eventStore"] as EventStoreConfiguration);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        Try<EventStoreConfiguration[]> GetAllConfigurations()
        {
            var tenants = _resources.Keys.Select(_ => new TenantId(_));
            if (!tenants.Any())
            {
                return new NoTenantsConfigured();
            }
            var configurations = tenants.Select(GetConfiguration);
            if (configurations.Any(_ => !_.Success))
            {
                return configurations.First(_ => !_.Success).Exception;
            }
            return configurations.Select(_ => _.Result).ToArray();
        }
    }
}
