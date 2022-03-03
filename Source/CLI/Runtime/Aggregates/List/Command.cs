// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Aggregates.Management;
using Dolittle.Runtime.CLI.Configuration.Files;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Domain.Tenancy;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.List;

/// <summary>
/// The "dolittle runtime aggregates list" command.
/// </summary>
[Command("list", Description = "Lists all registered Aggregate Roots")]
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
    /// The entrypoint for the "dolittle runtime aggregates list" command.
    /// </summary>
    /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
    public async Task OnExecuteAsync(CommandLineApplication cli)
    {
        var runtimeAddress = await SelectRuntimeToConnectTo(cli);
        if (!runtimeAddress.Success)
        {
            return;
        }
        var aggregateRoots = await _client.GetAll(runtimeAddress, Tenant).ConfigureAwait(false);

        if (Output == OutputType.Json)
        {
            await WriteOutput(cli, aggregateRoots).ConfigureAwait(false);
        }
        else
        {
            await WriteTableOutput(cli, aggregateRoots).ConfigureAwait(false);
        }
    }
        
    Task WriteTableOutput(CommandLineApplication cli, IEnumerable<AggregateRootWithTenantScopedInstances> roots)
        => Wide
            ? WriteDetailedOutput(cli, roots)
            : WriteSimpleOutput(cli, roots);

    Task WriteSimpleOutput(CommandLineApplication cli, IEnumerable<AggregateRootWithTenantScopedInstances> roots)
        => WriteOutput(cli, roots.Select(CreateSimpleView));

    Task WriteDetailedOutput(CommandLineApplication cli, IEnumerable<AggregateRootWithTenantScopedInstances> roots)
        => WriteOutput(cli, roots.Select(CreateDetailedView));

    static AggregateRootSimpleView CreateSimpleView(AggregateRootWithTenantScopedInstances root)
        => new(
            root.AggregateRoot.Alias.Equals(AggregateRootAlias.NotSet)
                ? root.AggregateRoot.Identifier.Id.Value.ToString()
                : root.AggregateRoot.Alias.Value,
            (ulong)root.Aggregates.LongCount());

    static AggregateRootDetailedView CreateDetailedView(AggregateRootWithTenantScopedInstances root)
        => new(root.AggregateRoot.Alias.Value, root.AggregateRoot.Identifier.Id.Value, (ulong)root.Aggregates.LongCount());

}
