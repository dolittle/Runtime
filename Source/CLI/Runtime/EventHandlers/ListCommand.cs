// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Threading.Tasks;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// The "dolittle runtime eventhandlers list" command.
    /// </summary>
    [Command("list", "ls",  "List all running Event Handlers")]
    public class ListCommand : CommandBase
    {
        readonly IManagementClient _client;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommand"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="client">The management client to use.</param>
        /// <param name="serializer">The json <see cref="ISerializer"/>.</param>
        public ListCommand(ICanLocateRuntimes runtimes, IManagementClient client, ISerializer serializer)
            : base(runtimes)
        {
            _client = client;
            _serializer = serializer;
        }

        /// <summary>
        /// The entrypoint for the "dolittle runtime eventhandlers" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            var runtimeAddress = await SelectRuntimeToConnectTo(cli);
            if (!runtimeAddress.Success)
            {
                return;
            }
            var eventHandlerStatuses = await _client.GetAll(runtimeAddress).ConfigureAwait(false);
            var json = _serializer.ToJson(eventHandlerStatuses, SerializationOptions.Custom(
                SerializationOptionsFlags.None,
                callback: _ =>
            {
                _.Formatting = Formatting.Indented;
            }));
            // var json = JsonConvert.SerializeObject(eventHandlerStatuses, Formatting.Indented);
            await cli.Out.WriteAsync(json).ConfigureAwait(false);
        }
    }
}
