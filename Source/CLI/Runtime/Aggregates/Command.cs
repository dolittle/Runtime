// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.CLI.Serialization;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// The "dolittle runtime aggregates" command.
/// </summary>
[Command("aggregates", Description = "Manage Aggregates")]
[Subcommand(typeof(Events.Command))]
[Subcommand(typeof(List.Command))]
[Subcommand(typeof(Get.Command))]
public class Command : CommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="aggregateRootIdResolver">The <see cref="IResolveAggregateRootId"/>.</param>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    public Command(ICanLocateRuntimes runtimes, IResolveAggregateRootId aggregateRootIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, aggregateRootIdResolver, eventTypesDiscoverer, jsonSerializer)
    {
    }
        
    /// <summary>
    /// The entrypoint for the "dolittle runtime aggregates" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public void OnExecute(CommandLineApplication cli)
        => cli.ShowHelp();
}
