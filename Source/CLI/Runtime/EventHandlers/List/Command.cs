// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Serialization;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Domain.Tenancy;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.List;

/// <summary>
/// The "dolittle runtime eventhandlers list" command.
/// </summary>
[Command("list", "ls", Description = "List all running Event Handlers")]
public class Command : CommandBase
{
    readonly IManagementClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="client">The management client to use.</param>
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
    /// The entrypoint for the "dolittle runtime eventhandlers list" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }
        var eventHandlerStatuses = await _client.GetAll(runtimeAddress, Tenant).ConfigureAwait(false);
            
        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, eventHandlerStatuses).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, eventHandlerStatuses.ToList()).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, IEnumerable<EventHandlerStatus> eventHandlerStatuses)
        => Wide
            ? WriteOutput(cli, eventHandlerStatuses.Select(CreateDetailedView))
            : WriteOutput(cli, eventHandlerStatuses.Select(CreateSimpleView));
    
    static EventHandlerSimpleView CreateSimpleView(EventHandlerStatus status)
        => status.Partitioned switch
        {
            true => CreateSimpleView(status, status.States.Cast<PartitionedTenantScopedStreamProcessorStatus>().Any(_ => _.FailingPartitions.Any())),
            _ => CreateSimpleView(status, status.States.Cast<UnpartitionedTenantScopedStreamProcessorStatus>().Any(_ => _.IsFailing)),
        };

    static EventHandlerSimpleView CreateSimpleView(EventHandlerStatus status, bool isFailing)
        => new(
            status.HasAlias ? status.Alias : status.Id.EventHandler.Value.ToString(), 
            status.IsInDefaultScope ? "Default" : status.Id.Scope.Value.ToString(),
            status.Partitioned ? "✅" : "❌", 
            isFailing  ? "❌" : "✅");

    static EventHandlerDetailedView CreateDetailedView(EventHandlerStatus status)
    {
        if (status.Partitioned)
        {
            var states = status.States.Cast<PartitionedTenantScopedStreamProcessorStatus>().ToArray();
            return CreateDetailedView(
                status,
                states.Any(_ => _.FailingPartitions.Any()),
                states.Any() ? states.Max(_ => _.LastSuccessfullyProcessed) : DateTimeOffset.MinValue);
        }
        else
        {
            var states = status.States.Cast<UnpartitionedTenantScopedStreamProcessorStatus>().ToArray();
            return CreateDetailedView(
                status,
                states.Any(_ => _.IsFailing),
                states.Any() ? states.Max(_ => _.LastSuccessfullyProcessed) : DateTimeOffset.MinValue);
        }
    }

    static EventHandlerDetailedView CreateDetailedView(EventHandlerStatus status, bool isFailing, DateTimeOffset lastSuccessfullyProcessed)
        => new(
            status.Alias,
            status.Id.EventHandler,
            status.IsInDefaultScope ? "Default" : status.Id.Scope.Value.ToString(),
            status.Partitioned ? "✅" : "❌", 
            isFailing  ? "❌" : "✅",
            lastSuccessfullyProcessed);
}
