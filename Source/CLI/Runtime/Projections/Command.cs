// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// The "dolittle runtime projections" command.
/// </summary>
[Command("projections", Description = "Manage Projections")]
[Subcommand(typeof(List.Command))]
[Subcommand(typeof(Get.Command))]
public class Command: CommandBase
{
    public Command(ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
    }

    /// <summary>
    /// The entrypoint for the "dolittle runtime projections" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public void OnExecute(CommandLineApplication cli)
        => cli.ShowHelp();
}
