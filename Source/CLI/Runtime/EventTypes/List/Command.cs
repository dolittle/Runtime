// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes.List
{
    /// <summary>
    /// The "dolittle runtime eventtypes list" command.
    /// </summary>
    [Command("list", Description = "Lists all registered Event Types")]
    public class Command : CommandBase
    {
        readonly IManagementClient _client;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="client">The management client to use.</param>
        /// <param name="serializer">The json <see cref="ISerializer"/>.</param>
        public Command(ICanLocateRuntimes runtimes, IManagementClient client,  ISerializer serializer)
            : base(runtimes, serializer)
        {
            _client = client;
        }

        /// <summary>
        /// The entrypoint for the "dolittle runtime eventtypes list" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            var runtimeAddress = await SelectRuntimeToConnectTo(cli);
            if (!runtimeAddress.Success)
            {
                return;
            }
            var aggregateRoots = await _client.GetAll(runtimeAddress).ConfigureAwait(false);

            if (Output == OutputType.Json)
            {
                await WriteOutput(cli, aggregateRoots).ConfigureAwait(false);
            }
            else
            {
                await WriteTableOutput(cli, aggregateRoots).ConfigureAwait(false);
            }
        }
        
        Task WriteTableOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
            => Wide
                ? WriteDetailedOutput(cli, eventTypes)
                : WriteSimpleOutput(cli, eventTypes);

        Task WriteSimpleOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
            => WriteOutput(cli, eventTypes.Select(CreateSimpleView));

        Task WriteDetailedOutput(CommandLineApplication cli, IEnumerable<EventType> eventTypes)
            => WriteOutput(cli, eventTypes.Select(CreateDetailedView));

        static EventTypeSimpleView CreateSimpleView(EventType eventType)
            => new(eventType.Alias.Equals(EventTypeAlias.NotSet) ? eventType.Identifier.Id.Value.ToString() : eventType.Alias.Value);

        static EventTypeDetailedView CreateDetailedView(EventType eventType)
            => new(eventType.Alias, eventType.Identifier.Id);

    }
}
