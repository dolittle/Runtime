// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// The "dolittle runtime eventhandlers list" command.
    /// </summary>
    [Command("list", "ls",  "List all running Event Handlers")]
    public class ListCommand : CommandBase
    {
        readonly IManagementClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommand"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="client">The management client to use.</param>
        public ListCommand(ICanLocateRuntimes runtimes, IManagementClient client)
            : base(runtimes)
        {
            _client = client;
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
            // TODO: This should return the states
            await _client.GetAll(runtimeAddress).ConfigureAwait(false);
            // TODO: Print state
        }
    }
}
