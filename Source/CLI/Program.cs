// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CLI.Configurations;
using CLI.Options.Parsers.Versioning;
using Dolittle.Runtime.Migrations;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MigrateFrom = Dolittle.Runtime.Migrations;

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
            using var cli = new CommandLineApplication<Program>();
            var services = new ServiceCollection();
            BindServices(cli, services);
            var container = services.BuildServiceProvider();

            cli.Conventions.UseDefaultConventions();
            cli.Conventions.UseConstructorInjection(container);
            AddValueParsers(cli.ValueParsers, container);

            return cli.Execute(args);
        }

        static void BindServices(CommandLineApplication<Program> cli, ServiceCollection services)
        {
            services.AddSingleton<RuntimeConfigurationDirectoryPath>(cli.WorkingDirectory);
            services.AddTransient<IVersionConverter, VersionConverter>();
            services.AddTransient<IValueParser, VersionParser>();
            services.AddTransient<IMigrations, MigrateFrom.Migrations>();
            services.AddTransient<IPerformMigrations, MigrationPerformer>();
            services.AddTransient<ICanMigrateDataStores, MigrateFrom.V6.ToV7>();
            services.AddTransient<IConfigurations, Configurations.Configurations>();
            services.AddTransient<IResources, Resources>();
            services.AddSingleton<ISerializer>(new Serializer(new NoConverterProviders()));
            services.AddTransient<IMigrationPerformers, MigrationPerformers>();
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
