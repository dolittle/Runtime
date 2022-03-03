// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Configuration.Files;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Projections.Get;

/// <summary>
/// The "dolittle runtime projections get" command.
/// </summary>
[Command("get", Description = "Gets a running Projection")]
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
    /// The "--tenant" argument used to provide a Tenant Id.
    /// </summary>
    [Option("--tenant", CommandOptionType.SingleValue, Description = "Only show Projection information for the specified Tenant")]
    TenantId Tenant { get; init; }
    
    /// <summary>
    /// The Projection identifier argument used to provide the identifier of the Projection to get.
    /// </summary>
    [Required]
    [Argument(0, Description = "The identifier or alias of the Projection to get details for")]
    string IdentifierOrAlias { get; init; }
    
    /// <summary>
    /// The Projection scope argument used to provide the identifier of the Projection to get.
    /// </summary>
    [Argument(1, Description = "The scope of the Projection to get details for")]
    ScopeId Scope { get; init; }

    /// <summary>
    /// The entrypoint for the "dolittle runtime eventhandlers get" command.
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
        var getStatus = await _client.Get(runtimeAddress, scopeId, projectionId, Tenant).ConfigureAwait(false);

        if (!getStatus.Success)
        {
            throw getStatus.Exception;
        }

        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, getStatus.Result).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, getStatus.Result).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, ProjectionStatus status)
        => Wide
            ? WriteOutput(cli, status.States.Select(CreateDetailedStateView))
            : WriteOutput(cli, status.States.Select(CreateSimpleStateView));
    
    static ProjectionSimpleView CreateSimpleStateView(UnpartitionedTenantScopedStreamProcessorStatus status)
        => new(
            status.TenantId,
            status.Position,
            status.IsFailing ? "❌" : "✅");
    
    static ProjectionDetailedView CreateDetailedStateView(UnpartitionedTenantScopedStreamProcessorStatus status)
        => new(
            status.TenantId,
            status.Position, 
            status.IsFailing ? "❌" : "✅",
            status.FailureReason,
            status.RetryTime,
            status.ProcessingAttempts,
            status.LastSuccessfullyProcessed);
}
