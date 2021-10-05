// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CLI.Migrations;
using McMaster.Extensions.CommandLineUtils;

namespace CLI
{
    [Command("migrate", "Run migrations of data stores between major Runtime versions")]
    [Subcommand(typeof(MigrateEventStore))]
    public class Migrate
    {
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}