// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store.MongoDB;
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
                var configs = GetConfigs(tenant);
                if (!configs.Success)
                {
                    return Task.FromResult(Try.Failed(configs.Exception));
                }
                return configs.Result.Item2 == default
                    ? eventStoreMigration.Migrate(configs.Result.Item1)
                    : eventStoreMigration.Migrate(configs.Result.Item1, configs.Result.Item2);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Try.Failed(ex));
            }
        }

            /// <inheritdoc/>
        public async Task<Try> PerformForAllTenants(ICanMigrateAnEventStore eventStoreMigration)
        {
            var tenants = GetAllTenants();
            if (!tenants.Success)
            {
                return tenants.Exception;
            }
            var configs = new List<(EventStoreConfiguration, EmbeddingsConfiguration)>();
            foreach (var tenant in tenants.Result)
            {
                var eventStoreAndEmbeddingConfig = GetConfigs(tenant);
                if (!eventStoreAndEmbeddingConfig.Success)
                {
                    return eventStoreAndEmbeddingConfig;
                }
                configs.Add(eventStoreAndEmbeddingConfig);
            }
            foreach (var (eventStoreConfiguration, embeddingsConfiguration) in configs)
            {
                if (embeddingsConfiguration == default)
                {
                    var result = await eventStoreMigration.Migrate(eventStoreConfiguration).ConfigureAwait(false);
                    if (!result.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await eventStoreMigration.Migrate(eventStoreConfiguration, embeddingsConfiguration).ConfigureAwait(false);
                    if (!result.Success)
                    {
                        return result;
                    }
                }
            }
            return Try.Succeeded();
        }

        Try<IEnumerable<TenantId>> GetAllTenants()
        {
            try
            {
                var tenants = _configuration.Keys.Select(_ => new TenantId(_));
                return !tenants.Any() ? new NoTenantsConfigured() : Try<IEnumerable<TenantId>>.Succeeded(tenants);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        Try<(EventStoreConfiguration, EmbeddingsConfiguration)> GetConfigs(TenantId tenantId)
        {
            // This is horrible stuff...
            EventStoreConfiguration eventStoreConfiguration = default;
            EmbeddingsConfiguration embeddingsConfiguration = default;
            var result = (eventStoreConfiguration, embeddingsConfiguration);
            try
            {
                result.eventStoreConfiguration = _resourceConfigurationProvider.ConfigurationFor<EventStoreConfiguration>(tenantId, "eventStore");
            }
            catch (Exception ex)
            {
                return ex;
            }
            try
            {
                result.embeddingsConfiguration = _resourceConfigurationProvider.ConfigurationFor<EmbeddingsConfiguration>(tenantId, "embeddings");
            }
            catch (MissingResourceConfigurationForResourceTypeForTenant ex)
            {
                result.embeddingsConfiguration = default;
            }
            catch (Exception ex)
            {
                return ex;
            }
            return result;
        }
    }
}
