// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.CLI.Configuration.Files;
using Dolittle.Runtime.CLI.Configuration.Runtime;
using Dolittle.Runtime.CLI.Options.Parsers;
using Dolittle.Runtime.CLI.Runtime;
using Dolittle.Runtime.CLI.Serialization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.CLI
{
    /// <summary>
    /// The main entrypoint of the Dolittle CLI tool.
    /// </summary>
    [Command("dolittle", Description = "The Dolittle CLI tool")]
    [Subcommand(typeof(Runtime.Command))]
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                using var cli = new CommandLineApplication<Program>();
                var services = new ServiceCollection();
                
                services.AddSingleton<CommandLineApplication>(cli);
                
                AddServices(services);
                
                var container = services.BuildServiceProvider();
                
                cli.Conventions.UseDefaultConventions();
                cli.Conventions.UseConstructorInjection(container);
                cli.ValueParsers.UseAllValueParsers(container);

                return cli.Execute(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }

        static void AddServices(ServiceCollection services)
        {
            services.AddValueParsers();
            services.AddSerializers();
            
            services.AddConfigurationFiles();
            services.AddRuntimeConfiguration();
            services.AddRuntimeServices();
        }

        /// <summary>
        /// The main entrypoint of the "dolittle" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}
