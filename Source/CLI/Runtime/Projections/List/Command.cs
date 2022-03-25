// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Serialization;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Domain.Tenancy;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Projections.List;

/// <summary>
/// The "dolittle runtime projections list" command.
/// </summary>
[Command("list", "ls", Description = "List all running Projections")]
public class Command : CommandBase
{
    readonly IManagementClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="client">The management client to use to get all registered Projections.</param>
    /// <param name="runtimes">The locator to use to find the Runtime to connect to.</param>
    /// <param name="eventTypesDiscoverer">The event types discoverer to use to discover event types.</param>
    /// <param name="jsonSerializer">The JSON serializer to use.</param>
    public Command(IManagementClient client, ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
        _client = client;
    }
        
    /// <summary>
    /// The "--tenant" argument used to provide a Tenant Id.
    /// </summary>
    [Option("--tenant", CommandOptionType.SingleValue, Description = "Only show Projection information for the specified Tenant")]
    TenantId Tenant { get; init; }

    /// <summary>
    /// The entrypoint for the "dolittle runtime projections list" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }
        var projectionStatuses = await _client.GetAll(runtimeAddress, Tenant).ConfigureAwait(false);

        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, projectionStatuses).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, projectionStatuses).ConfigureAwait(false);
        }
    }
    
    Task WriteTableOutput(CommandLineApplication cli, IEnumerable<ProjectionStatus> projectionStatuses)
        => Wide
            ? WriteOutput(cli, projectionStatuses.Select(CreateDetailedView))
            : WriteOutput(cli, projectionStatuses.Select(CreateSimpleView));
    
    static ProjectionSimpleView CreateSimpleView(ProjectionStatus status)
        => new(
            status.HasAlias ? status.Alias : status.Id.Value.ToString(), 
            status.IsInDefaultScope ? "Default" : status.Scope.Value.ToString(),
            status.States.Any(_ => _.IsFailing) ? "❌" : "✅",
            status.Copies.MongoDB.ShouldCopyToMongoDB ? "✅" : " ");

    static ProjectionDetailedView CreateDetailedView(ProjectionStatus status)
        => new(
            status.Alias,
            status.Id,
            status.IsInDefaultScope ? "Default" : status.Scope.Value.ToString(),
            status.States.Any(_ => _.IsFailing) ? "❌" : "✅",
            status.States.Any() ? status.States.Max(_ => _.LastSuccessfullyProcessed) : DateTimeOffset.MinValue,
            status.Copies.MongoDB.ShouldCopyToMongoDB ? "✅" : " ");
}
