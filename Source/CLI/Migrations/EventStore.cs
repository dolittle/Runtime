// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    /// <summary>
    /// The "dolittle migrate eventstore" command.
    /// </summary>
    [Command("eventstore", "Run migrations of event store between major Runtime versions")]
    public class MigrateEventStore : Options
    {
        /// <summary>
        /// The entrypoint of the "dolittle migrate eventstore" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}