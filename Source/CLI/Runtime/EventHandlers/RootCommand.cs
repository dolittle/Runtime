// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// The "dolittle runtime eventhandlers" command.
    /// </summary>
    [Command("eventhandlers", "Manage Event Handlers")]
    [Subcommand(typeof(ReplayCommand))]
    public class RootCommand : CommandBase
    {
        /// <summary>
        /// The entrypoint for the "dolittle runtime eventhandlers" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}
