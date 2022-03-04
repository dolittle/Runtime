// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Serialization;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Replay;

/// <summary>
/// The "dolittle runtime eventhandlers replay all" command.
/// </summary>
[Command("all", Description = "Make an Event Handler reprocess all events")]
public class AllCommand : CommandBase
{
    readonly IManagementClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllCommand"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="client">The management client to use.</param>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    public AllCommand(ICanLocateRuntimes runtimes, IManagementClient client, IResolveEventHandlerId eventHandlerIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventHandlerIdResolver, eventTypesDiscoverer, jsonSerializer)
    {
        _client = client;
    }

    /// <summary>
    /// The entrypoint for the "dolittle runtime eventhandlers replay all" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var address = await SelectRuntimeToConnectTo(cli);
        if (!address.Success)
        {
            return;
        }

        await _client.ReprocessAllEvents(await GetEventHandlerId(address, EventHandlerIdentifier).ConfigureAwait(false), address);
    }
}
