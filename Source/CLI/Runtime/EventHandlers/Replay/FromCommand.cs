// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Replay
{
    /// <summary>
    /// The "dolittle runtime eventhandlers replay from" command.
    /// </summary>
    [Command("from", "Make an Event Handler reprocess events from a specific position for a tenant")]
    public class FromCommand : CommandBase
    {
        readonly IManagementClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromCommand"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="client">The management client to use.</param>
        public FromCommand(ICanLocateRuntimes runtimes, IManagementClient client)
            : base(runtimes)
        {
            _client = client;
        }
        
        [Required]
        [Argument(0, Description = "The position to start replaying events from.")]
        StreamPosition Position { get; init; }
        
        [Required]
        [Option("--tenant", CommandOptionType.SingleValue, Description = "The tenant to replay events for. Defaults to the development tenant.")]
        TenantId Tenant { get; init; }

        /// <summary>
        /// The entrypoint for the "dolittle runtime eventhandlers replay from" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            var address = await SelectRuntimeToConnectTo(cli);
            if (!address.Success)
            {
                return;
            }

            await _client.ReprocessEventsFrom(SpecifiedScopeOrDefault, Identifier, Tenant, Position, address);
        }
    }
}
