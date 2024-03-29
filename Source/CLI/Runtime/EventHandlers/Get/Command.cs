// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Serialization;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Domain.Tenancy;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Get;

/// <summary>
/// The "dolittle runtime eventhandlers get" command.
/// </summary>
[Command("get", Description = "Gets a running Event Handler")]
public class Command : CommandBase
{
    readonly IManagementClient _client;
        

    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="client">The management client to use.</param>
    /// <param name="eventHandlerIdResolver">The Event Handler Id resolver.</param>
    /// <param name="serializer">The json <see cref="ISerializer"/>.</param>
    public Command(ICanLocateRuntimes runtimes, IManagementClient client, IResolveEventHandlerId eventHandlerIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer serializer)
        : base(runtimes, eventHandlerIdResolver, eventTypesDiscoverer, serializer)
    {
        _client = client;
    }
        
    /// <summary>
    /// The "--tenant" argument used to provide a Tenant Id.
    /// </summary>
    [Option("--tenant", CommandOptionType.SingleValue, Description = "Only show Event Handler information for the specified Tenant")]
    TenantId? Tenant { get; init; }
        
    /// <summary>
    /// The Event Handler identifier argument used to provide the unique identifier of the Event Handler to get.
    /// </summary>
    [Required]
    [Argument(0, Description = "The Event Handler identifier of the Event Handler to get details for")]
    EventHandlerIdOrAlias EventHandlerIdentifier { get; init; }

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
        var getStatus = await _client.Get(runtimeAddress, await GetEventHandlerId(runtimeAddress, EventHandlerIdentifier).ConfigureAwait(false), Tenant).ConfigureAwait(false);

        if (!getStatus.Success)
        {
            throw getStatus.Exception;
        }

        var status = getStatus.Result;
            
            
        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, status).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, status).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, EventHandlerStatus status)
        => Wide
            ? WriteDetailedOutput(cli, status)
            : WriteSimpleOutput(cli, status);

    Task WriteSimpleOutput(CommandLineApplication cli, EventHandlerStatus status)
        => status.Partitioned
            ? WriteOutput(cli, status.States.Cast<PartitionedTenantScopedStreamProcessorStatus>().Select(CreateSimpleStateView))
            : WriteOutput(cli, status.States.Cast<UnpartitionedTenantScopedStreamProcessorStatus>().Select(CreateSimpleStateView));

    Task WriteDetailedOutput(CommandLineApplication cli, EventHandlerStatus status)
        => status.Partitioned
            ? WriteOutput(cli, MergePartitionedStates(status))
            : WriteOutput(cli, status.States.Cast<UnpartitionedTenantScopedStreamProcessorStatus>().Select(CreateDetailedStateView));

    static IEnumerable<PartitionedEventHandlerDetailedView> MergePartitionedStates(EventHandlerStatus status)
    {
        var states = status.States.Cast<PartitionedTenantScopedStreamProcessorStatus>();
        var views = new List<PartitionedEventHandlerDetailedView>();

        foreach (var state in states)
        {
            views.Add(CreateDetailedStateView(state));
            if (state.FailingPartitions.Any())
            {
                views.AddRange(state.FailingPartitions.Select(_ => CreateDetailedStateView(state, _)));
            }
        }
        return views;
    }

    static EventHandlerSimpleView CreateSimpleStateView(UnpartitionedTenantScopedStreamProcessorStatus status)
        => new(status.TenantId, status.Position, status.IsFailing ? "❌" : "✅");
        
    static UnpartitionedEventHandlerDetailedView CreateDetailedStateView(UnpartitionedTenantScopedStreamProcessorStatus status)
        => new(
            status.TenantId,
            status.Position, 
            status.IsFailing ? "❌" : "✅",
            status.FailureReason,
            status.RetryTime,
            status.ProcessingAttempts,
            status.LastSuccessfullyProcessed);

    static EventHandlerSimpleView CreateSimpleStateView(PartitionedTenantScopedStreamProcessorStatus status)
        => new(status.TenantId, status.Position, status.FailingPartitions.Any() ? "❌" : "✅");

    static PartitionedEventHandlerDetailedView CreateDetailedStateView(PartitionedTenantScopedStreamProcessorStatus status, FailingPartition failingPartition = null)
        => new(
            status.TenantId,
            failingPartition?.Position ?? status.Position,
            failingPartition != null ? "❌" : status.FailingPartitions.Any() ? "🟡️" : "✅",
            failingPartition?.LastFailed ?? status.LastSuccessfullyProcessed,
            failingPartition?.Id,
            failingPartition?.FailureReason,
            failingPartition?.RetryTime ?? default,
            failingPartition?.ProcessingAttempts ?? 0);
}
