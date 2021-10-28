// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Events
{
    /// <summary>
    /// The "dolittle runtime aggregates events" command.
    /// </summary>
    [Command("events", Description = "Gets all committed aggregate events for an aggregate root instance")]
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
        public Command(ICanLocateRuntimes runtimes, IManagementClient client, IResolveAggregateRootId aggregateRootIdResolver,  ISerializer serializer)
            : base(runtimes, aggregateRootIdResolver, serializer)
        {
            _client = client;
        }

        /// <summary>
        /// The Aggregate Root identifier argument used to provide the unique identifier of the Aggregate Root to get information for.
        /// </summary>
        [Required]
        [Argument(0, Description = "The Aggregate Root identifier of the Aggregate Root to get committed aggregate events for.")]
        AggregateRootIdOrAlias AggregateRootIdentifier { get; init; }

        /// <summary>
        /// The Event Source Id argument used to provide the event source id of the Aggregate Root Instance to get committed aggregate events for.
        /// </summary>
        [Required]
        [Argument(1, Description = "The Event Source Id of the Aggregate Root Instance to get committed aggregate events for.")]
        EventSourceId EventSourceId { get; init; }
        
        /// <summary>
        /// The Tenant Id argument used to provide the tenant to get the committed aggregate events from.
        /// </summary>
        [Required]
        [Argument(2, Description = "The Tenant Id to get the committed aggregate events from.")]
        TenantId Tenant { get; init; }

        /// <summary>
        /// The entrypoint for the "dolittle runtime aggregates events" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            var runtimeAddress = await SelectRuntimeToConnectTo(cli);
            if (!runtimeAddress.Success)
            {
                return;
            }
            var events = await _client.GetEvents(runtimeAddress, await GetAggregateRootId(runtimeAddress, AggregateRootIdentifier).ConfigureAwait(false), EventSourceId, Tenant).ConfigureAwait(false);
            

            if (Output == OutputType.Json)
            {
                await WriteOutput(cli, events).ConfigureAwait(false);
            }
            else
            {
                await WriteTableOutput(cli, events).ConfigureAwait(false);
            }
        }
        
        Task WriteTableOutput(CommandLineApplication cli, CommittedAggregateEvents events)
            => Wide
                ? WriteDetailedOutput(cli, events)
                : WriteSimpleOutput(cli, events);

        Task WriteSimpleOutput(CommandLineApplication cli, CommittedAggregateEvents events)
            => WriteOutput(cli, CreateSimpleView(events));

        Task WriteDetailedOutput(CommandLineApplication cli, CommittedAggregateEvents events)
            => WriteOutput(cli, CreateDetailedView(events));

        static CommittedAggregateEventsSimpleView CreateSimpleView(CommittedAggregateEvents events)
            => new((ulong)events.Count);

        static CommittedAggregateEventsDetailedView CreateDetailedView(CommittedAggregateEvents events)
            => new((ulong)events.Count);
    }
}
