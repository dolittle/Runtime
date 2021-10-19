// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime
{
    /// <summary>
    /// The "dolittle runtime" command.
    /// </summary>
    [Command("runtime", Description = "Manage a Runtime")]
    [Subcommand(typeof(EventHandlers.Command))]
    public class Command : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        public Command(ICanLocateRuntimes runtimes)
            : base(runtimes)
        {
        }
        
        /// <summary>
        /// The entrypoint for the "dolittle runtime" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}