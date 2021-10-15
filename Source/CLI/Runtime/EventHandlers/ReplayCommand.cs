// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Microservices;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// The "dolittle runtime eventhandlers replay" command.
    /// </summary>
    [Command("replay", "Make an Event Handler reprocess events")]
    public class ReplayCommand : CommandBase
    {
        readonly ICanLocateRuntimes _runtimeLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplayCommand"/> class.
        /// </summary>
        /// <param name="runtimeLocator">The Runtime locator to find a Runtime to connect to.</param>
        public ReplayCommand(ICanLocateRuntimes runtimeLocator)
        {
            _runtimeLocator = runtimeLocator;
        }

        /// <summary>
        /// The entrypoint for the "dolittle runtime eventhandlers replay" command.
        /// </summary>
        /// <param name="cli">The <see cref="CommandLineApplication"/> that is executed.</param>
        public async Task OnExecuteAsync(CommandLineApplication cli)
        {
            var runtimeAddresses = (await _runtimeLocator.GetAvailableRuntimeAddresses(Runtime)).ToList();

            if (!runtimeAddresses.Any())
            {
                cli.Error.WriteLine("Could not find any Runtimes to connect to locally, please ensure you have one running and if so specify the address manually using --runtime <host[:port]>.");
                return;
            }

            var address = SelectRuntimeAddress(cli, runtimeAddresses);

            
            cli.Out.WriteLine("Replaying events");
            cli.Out.WriteLine(address);
        }

        MicroserviceAddress SelectRuntimeAddress(CommandLineApplication cli, IList<MicroserviceAddress> addresses)
        {
            if (addresses.Count == 1)
            {
                return addresses[0];
            }

            cli.Out.WriteLine("Found multiple available Runtimes, please select one of the following:");
            while (true)
            {
                for (var i = 0; i < addresses.Count; i++)
                {
                    var (host, port) = addresses[i];
                    cli.Out.WriteLine($"\t{i}) {host.Value}:{port.Value}");
                }

                var selection = Prompt.GetInt($"Select Runtime (0-{addresses.Count - 1}):");

                if (selection >= 0 && selection < addresses.Count)
                {
                    return addresses[selection];
                }
                
                cli.Out.WriteLine("Invalid number, please select one of the following:");
            }
        }
    }
}
