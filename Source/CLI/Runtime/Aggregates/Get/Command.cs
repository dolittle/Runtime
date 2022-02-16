// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Get;

/// <summary>
/// The "dolittle runtime aggregates get" command.
/// </summary>
[Command("get", Description = "Gets a registered Aggregate Root")]
public class Command : CommandBase
{
    readonly IManagementClient _client;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="client">The management client to use.</param>
    /// <param name="aggregateRootIdResolver">The Aggregate Root Id resolver.</param>
    /// <param name="serializer">The json <see cref="ISerializer"/>.</param>
    public Command(ICanLocateRuntimes runtimes, IManagementClient client, IResolveAggregateRootId aggregateRootIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer serializer)
        : base(runtimes, aggregateRootIdResolver, eventTypesDiscoverer, serializer)
    {
        _client = client;
    }
        
    /// <summary>
    /// The "--tenant" argument used to provide a Tenant Id.
    /// </summary>
    [Option("--tenant", CommandOptionType.SingleValue, Description = "Only show Aggregate Root information for the specified Tenant")]
    TenantId Tenant { get; init; }
        
    /// <summary>
    /// The Aggregate Root identifier argument used to provide the unique identifier of the Aggregate Root to get information for.
    /// </summary>
    [Required]
    [Argument(0, Description = "The Aggregate Root identifier of the Aggregate Root to get details for")]
    AggregateRootIdOrAlias AggregateRootIdentifier { get; init; }

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
        var getAggregateRoot = await _client.Get(runtimeAddress, await GetAggregateRootId(runtimeAddress, AggregateRootIdentifier).ConfigureAwait(false), Tenant).ConfigureAwait(false);

        if (!getAggregateRoot.Success)
        {
            throw getAggregateRoot.Exception;
        }

        var aggregateRoot = getAggregateRoot.Result;
            
            
        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, aggregateRoot).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, aggregateRoot).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, AggregateRootWithTenantScopedInstances root)
        => Wide
            ? WriteDetailedOutput(cli, root)
            : WriteSimpleOutput(cli, root);

    Task WriteSimpleOutput(CommandLineApplication cli, AggregateRootWithTenantScopedInstances root)
        => WriteOutput(cli, root.Aggregates.Select(CreateSimpleView));

    Task WriteDetailedOutput(CommandLineApplication cli, AggregateRootWithTenantScopedInstances root)
        => WriteOutput(cli, root.Aggregates.Select(CreateDetailedView));

    static AggregateRootInstanceSimpleView CreateSimpleView(TenantScopedAggregateRootInstance instance)
        => new(instance.Instance.EventSource);

    static AggregateRootInstanceDetailedView CreateDetailedView(TenantScopedAggregateRootInstance instance)
        => new(instance.Tenant, instance.Instance.EventSource, instance.Instance.Version);
        

}
