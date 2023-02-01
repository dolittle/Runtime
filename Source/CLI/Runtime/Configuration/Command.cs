// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Runtime.Aggregates;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.CLI.Serialization;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Configuration;

/// <summary>
/// The "dolittle runtime config" command.
/// </summary>
[Command("config", Description = "Gets the configuration yaml")]
public class Command : CommandBase
{
    readonly IManagementClient _client;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="aggregateRootIdResolver">The <see cref="IResolveAggregateRootId"/>.</param>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    public Command(ICanLocateRuntimes runtimes, IManagementClient client, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
        _client = client;
    }
        
   
    /// <summary>
    /// The entrypoint for the "dolittle runtime aggregates get" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }
        var configYaml = await _client.GetConfigurationYaml(runtimeAddress).ConfigureAwait(false);
        await cli.Out.WriteAsync(configYaml).ConfigureAwait(false);
    }
}
