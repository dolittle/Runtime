// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
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
        readonly IPerformMigrations _performer;

        public MigrateEventStore(IMigrations migrations, IPerformMigrations performer)
        {
            _migrations = migrations;
            _performer = performer;
        }

        /// <summary>
        /// The entrypoint of the "dolittle migrate eventstore" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public Task OnExecuteAsync(CommandLineApplication cli)
        {
            var migrator = _migrations.GetFor(From, To);
            if (!migrator.Success)
            {
                cli.Error.WriteLine("No migration to perform from version {0} to {1}. {2}", From, To, migrator.Exception.Message);
                return Task.CompletedTask;
            }

            return _performer.PerformForAllTenants(migrator.Result.EventStore);
        }
    }
}