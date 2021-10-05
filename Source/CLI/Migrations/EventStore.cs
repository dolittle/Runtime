// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    [Command("eventstore", "Run migrations of event store between major Runtime versions")]
    public class MigrateEventStore : Options
    {
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}