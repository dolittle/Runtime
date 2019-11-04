/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using Dolittle.Booting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents a bootloader for booting a server
    /// </summary>
    public class Bootloader
    {
        /// <summary>
        /// Start booting the server
        /// </summary>
        public static async Task Start()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureLogging(_ =>
            {
                _.AddConsole();
            });
            hostBuilder.UseEnvironment("Development");
            var host = hostBuilder.Build();
            var loggerFactory = host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            var result = Dolittle.Booting.Bootloader.Configure(_ =>
            {
                _.UseLoggerFactory(loggerFactory);
                _.Development();
            }).Start();

            await host.RunAsync();
        }
    }
}