// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Projections.Replay;

/// <summary>
/// The "dolittle runtime projections replay" command.
/// </summary>
[Command("replay", Description = "Replay a Projection")]
public class Command : CommandBase
{
    readonly IManagementClient _client;
    readonly IResolveProjectionIdAndScope _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="client">The management client to use to get all registered Projections.</param>
    /// <param name="resolver">The resolver to use to resolve arguments to a Projection.</param>
    /// <param name="runtimes">The locator to use to find the Runtime to connect to.</param>
    /// <param name="eventTypesDiscoverer">The event types discoverer to use to discover event types.</param>
    /// <param name="jsonSerializer">The JSON serializer to use.</param>
    public Command(IManagementClient client, IResolveProjectionIdAndScope resolver, ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
        _client = client;
        _resolver = resolver;
    }
    
        
    /// <summary>
    /// The "--tenant" argument used to specify a single Tenant to replay a Projection for.
    /// </summary>
    [Option("--tenant", CommandOptionType.SingleValue, Description = "Only replay the Projection for a specific Tenant")]
    TenantId Tenant { get; init; }
    
    /// <summary>
    /// The Projection identifier argument used to provide the identifier of the Projection to replay.
    /// </summary>
    [Required]
    [Argument(0, Description = "The identifier or alias of the Projection to replay")]
    string IdentifierOrAlias { get; init; }
    
    /// <summary>
    /// The Projection scope argument used to provide the identifier of the Projection to replay.
    /// </summary>
    [Argument(1, Description = "The scope of the Projection to replay")]
    ScopeId Scope { get; init; }
    
    /// <summary>
    /// The entrypoint for the "dolittle runtime eventhandlers replay" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }

        var getIdentity= await _resolver.Resolve(runtimeAddress, IdentifierOrAlias, Scope);
        if (!getIdentity.Success)
        {
            throw getIdentity.Exception;
        }

        var ( projectionId, scopeId ) = getIdentity.Result;
        var replaying = await _client.Replay(runtimeAddress, scopeId, projectionId, Tenant).ConfigureAwait(false);

        if (!replaying.Success)
        {
            throw replaying.Exception;
        }
    }
}
