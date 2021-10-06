// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using CLI.Configurations;
using Dolittle.Runtime.Migrations;
using Dolittle.Runtime.ResourceTypes.Configuration;
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
        readonly IPerformMigrations _performer;
        readonly IResources _resources;

        public MigrateEventStore(IMigrations migrations, IPerformMigrations performer, IResources resources)
        {
            _migrations = migrations;
            _performer = performer;
            _resources = resources;
        }

        /// <summary>
        /// The entrypoint of the "dolittle migrate eventstore" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public Task OnExecuteAsync(CommandLineApplication cli)
        {
            var resources = _resources.TryGet(ResourcesConfigName);
            if (!resources.Success)
            {
                cli.Error.WriteLine("Could not read resources configuration file. {0}", resources.Exception.Message);
                return Task.CompletedTask;
            }
            var migrator = _migrations.GetFor(From, To);
            if (migrator.Success) return _performer.PerformForAllTenants(migrator.Result.EventStore);
            cli.Error.WriteLine("No migration to perform from version {0} to {1}. {2}", From, To, migrator.Exception.Message);
            return Task.CompletedTask;
        }

    }
}