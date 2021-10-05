// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CLI.Options.Parsers.Versioning;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CLI
{
    /// <summary>
    /// The main entrypoint of the Dolittle CLI tool.
    /// </summary>
    [Command("dolittle", "The Dolittle CLI tool")]
    [Subcommand(typeof(Migrate))]
    class Program
    {
        static int Main(string[] args)
        {
            var services = new ServiceCollection();
            BindServices(services);
            var container = services.BuildServiceProvider();

            using var cli = new CommandLineApplication<Program>();
            cli.Conventions.UseDefaultConventions();
            AddValueParsers(cli.ValueParsers, container);

            return cli.Execute(args);
        }

        static void BindServices(ServiceCollection services)
        {
            services.AddTransient<IVersionConverter, VersionConverter>();
            services.AddTransient<IValueParser, VersionParser>();
        }

        static void AddValueParsers(ValueParserProvider parsers, ServiceProvider container)
        {
            foreach (var parser in container.GetServices<IValueParser>())
            {
                parsers.Add(parser);
            }
        }

        /// <summary>
        /// The main entrypoint of the "dolittle" command.
        /// </summary>
        /// <param name="cli">The command line application.</param>
        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}
