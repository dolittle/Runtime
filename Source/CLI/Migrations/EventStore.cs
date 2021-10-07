// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using CLI.Configurations;
using Dolittle.Runtime.Migrations;
using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    /// <summary>
    /// The "dolittle migrate eventstore" command.
    /// </summary>
    [Command("eventstore", "Run migrations of event store between major Runtime versions")]
    public class MigrateEventStore : Options
    {
        readonly IMigrations _migrations;
        readonly IMigrationPerformers _performers;
        readonly IResources _resources;

        public MigrateEventStore(IMigrations migrations, IMigrationPerformers performers, IResources resources)
        {
            _migrations = migrations;
            _performers = performers;
            _resources = resources;
        }

        /// <summary>
        /// The entrypoint of the "dolittle migrate eventstore" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            if (!TryGetPerformer(cli, out var performer))
            {
                return;
            }
            if (!TryGetMigrator(cli, out var migrator))
            {
                return;
            }
            var result = await performer.PerformForAllTenants(migrator.EventStore).ConfigureAwait(false);
            if (!result.Success)
            {
                cli.Error.WriteLine("Failed to perform migration for all tenants from version {0} to {1}. {2}", From, To, result.Exception);
            }
        }

        bool TryGetPerformer(CommandLineApplication cli, out IPerformMigrations performer)
        {
            performer = null;
            var resources = _resources.TryGet(ResourcesConfigName);
            if (!resources.Success)
            {
                cli.Error.WriteLine("Could not read resources configuration file. {0}", resources.Exception.Message);
                return false;
            }
            performer = _performers.ConfiguredFor(resources.Result);
            return true;
        }
        bool TryGetMigrator(CommandLineApplication cli, out ICanMigrateDataStores migrator)
        {
            migrator = null;
            var getMigrator = _migrations.GetFor(From, To);
            if (!getMigrator.Success)
            {
                cli.Error.WriteLine("No migration to perform from version {0} to {1}. {2}", From, To, getMigrator.Exception.Message);
                return false;
            }
            migrator = getMigrator.Result;
            return true;
        }

    }
}