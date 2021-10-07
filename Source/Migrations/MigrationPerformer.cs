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
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// An implementation of <see cref="IPerformMigrations"/>.
    /// </summary>
    public class MigrationPerformer : IPerformMigrations
    {
        readonly ICanProvideResourceConfigurationsByTenant _resourceConfigurationProvider;
        readonly ResourceConfigurationsByTenant _configuration;

        public MigrationPerformer(ICanProvideResourceConfigurationsByTenant resourceConfigurationProvider, ResourceConfigurationsByTenant configuration)
        {
            _resourceConfigurationProvider = resourceConfigurationProvider;
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public Task<Try> PerformForTenant(ICanMigrateAnEventStore eventStoreMigration, TenantId tenant)
        {
            try
            {
                return eventStoreMigration.Migrate(_resourceConfigurationProvider.ConfigurationFor<EventStoreConfiguration>(tenant, "eventStore"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Try.Failed(ex));
            }
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

        Try<EventStoreConfiguration[]> GetAllConfigurations()
        {
            try
            {
                var tenants = _configuration.Keys.Select(_ => new TenantId(_));
                if (!tenants.Any())
                {
                    return new NoTenantsConfigured();
                }
                return tenants
                    .Select(_ => _resourceConfigurationProvider.ConfigurationFor<EventStoreConfiguration>(_, "eventStore"))
                    .ToArray();
            }
            catch (Exception ex)
            {
                return ex;
            }
            
        }
    }
}
