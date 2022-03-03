// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Configuration.Files;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.Events;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes.List;

/// <summary>
/// The "dolittle runtime eventtypes list" command.
/// </summary>
[Command("list", Description = "Lists all registered Event Types")]
public class Command : CommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="client">The management client to use.</param>
    /// <param name="serializer">The json <see cref="ISerializer"/>.</param>
    public Command(ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer,  ISerializer serializer)
        : base(runtimes, eventTypesDiscoverer, serializer)
    {
    }

    /// <summary>
    /// The entrypoint for the "dolittle runtime eventtypes list" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }
        await PopulateEventTypes(runtimeAddress).ConfigureAwait(false);

        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, EventTypes).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, EventTypes).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
        => Wide
            ? WriteDetailedOutput(cli, eventTypes)
            : WriteSimpleOutput(cli, eventTypes);

    Task WriteSimpleOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
        => WriteOutput(cli, eventTypes.Select(CreateSimpleView));

    Task WriteDetailedOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
        => WriteOutput(cli, eventTypes.Select(CreateDetailedView));

    static EventTypeSimpleView CreateSimpleView(EventType eventType)
        => new(eventType.Alias.Equals(EventTypeAlias.NotSet) ? eventType.Identifier.Id.Value.ToString() : eventType.Alias.Value);

    static EventTypeDetailedView CreateDetailedView(EventType eventType)
        => new(eventType.Alias, eventType.Identifier.Id);

}