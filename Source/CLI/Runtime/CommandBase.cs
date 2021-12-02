// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// A shared command base for the "dolittle runtime" commands that provides shared arguments.
/// </summary>
public abstract class CommandBase : CLI.CommandBase
{
    readonly ICanLocateRuntimes _runtimes;
    readonly IDiscoverEventTypes _eventTypesDiscoverer;
    IEnumerable<EventType> _eventTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="eventTypesDiscoverer">The system that can discover registered event types from the Runtime.</param>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    protected CommandBase(ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(jsonSerializer)
    {
        _runtimes = runtimes;
        _eventTypesDiscoverer = eventTypesDiscoverer;
    }

    /// <summary>
    /// The "--runtime" argument used to provide an address for to a Runtime to connect to.
    /// </summary>
    [Option("--runtime", CommandOptionType.SingleValue, Description = "The <host[:port]> to use to connect to the management endpoint of a Runtime")]
    MicroserviceAddress Runtime { get; init; }
        
    protected IEnumerable<EventType> EventTypes
    {
        get
        {
            if (_eventTypes == default)
            {
                    
            }
            return _eventTypes ?? throw new EventTypesNotPopulated();
        }
    }

    /// <summary>
    /// Prompts the user to select the address of the Runtime to connect to.
    /// </summary>
    /// <returns>A <see cref="Try{TResult}"/> of type <see cref="MicroserviceAddress"/>.</returns>
    protected async Task<Try<MicroserviceAddress>> SelectRuntimeToConnectTo(CommandLineApplication cli)
    {
        var addresses = (await _runtimes.GetAvailableRuntimeAddresses(Runtime)).ToList();

        if (!addresses.Any())
        {
            await cli.Error.WriteLineAsync("Could not find any Runtimes to connect to locally, please ensure you have one running and if so specify the address manually using --runtime <host[:port]>.");
            return new CouldNotFindRuntimeAddress();
        }

        if (addresses.Count == 1)
        {
            return addresses[0];
        }
            
        await cli.Out.WriteLineAsync("Found multiple available Runtimes, please select one of the following:");
        while (true)
        {
            for (var i = 0; i < addresses.Count; i++)
            {
                var (host, port) = addresses[i];
                await cli.Out.WriteLineAsync($"\t{i}) {host.Value}:{port.Value}");
            }

            var selection = Prompt.GetInt($"Select Runtime (0-{addresses.Count - 1}):");

            if (selection >= 0 && selection < addresses.Count)
            {
                return addresses[selection];
            }
                
            await cli.Out.WriteLineAsync("Invalid number, please select one of the following:");
        }
    }
        
    /// <summary>
    /// Populates the <see cref="EventTypes"/> property with event types discovered from the Runtime.
    /// </summary>
    protected async Task PopulateEventTypes(MicroserviceAddress runtime)
    {
        _eventTypes = await _eventTypesDiscoverer.Discover(runtime).ConfigureAwait(false);
    }
        
    /// <summary>
    /// Resolves the given event type id to the alias or id of the registered event type.
    /// </summary>
    protected string ResolveEventTypeIdentifier(ArtifactId eventTypeId)
    {
        var eventType = EventTypes.FirstOrDefault(_ => _.Identifier.Id.Equals(eventTypeId));
        if (eventType == default)
        {
            return eventTypeId.Value.ToString();
        }
        return eventType.Alias.Equals(EventTypeAlias.NotSet)
            ? eventTypeId.Value.ToString()
            : eventType.Alias;
    }
}