// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// The "dolittle runtime eventhandlers" command.
    /// </summary>
    [Command("eventhandlers", Description = "Manage Event Handlers")]
    [Subcommand(typeof(Replay.Command))]
    [Subcommand(typeof(List.Command))]
    [Subcommand(typeof(Get.Command))]
    public class Command : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
        public Command(ICanLocateRuntimes runtimes, ISerializer jsonSerializer)
            : base(runtimes, jsonSerializer)
        {
        }
        
        /// <summary>
        /// The entrypoint for the "dolittle runtime eventhandlers" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}
