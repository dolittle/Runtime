// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// The "dolittle runtime" command.
    /// </summary>
    [Command("runtime", "Manage a Runtime")]
    [Subcommand(typeof(EventHandlers.RootCommand))]
    public class RootCommand : CommandBase
    {
        /// <summary>
        /// The entrypoint for the "dolittle runtime" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}