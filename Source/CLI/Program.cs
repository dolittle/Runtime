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
using Microsoft.Extensions.Logging;
using EventStore = Dolittle.Runtime.Events.Store.MongoDB.Migrations;

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
            // while (!System.Diagnostics.Debugger.IsAttached) System.Threading.Thread.Sleep(50);
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
            services.AddLogging(_ => _.AddConsole());
            services.AddSingleton<RuntimeConfigurationDirectoryPath>(cli.WorkingDirectory);
            services.AddTransient<IVersionConverter, VersionConverter>();
            services.AddTransient<IValueParser, VersionParser>();
            services.AddTransient<IMigrations, Dolittle.Runtime.Migrations.Migrations>();
            services.AddTransient<IConfigurations, Configurations.Configurations>();
            services.AddTransient<IResources, Resources>();
            services.AddSingleton<ISerializer>(new Serializer(new NoConverterProviders()));
            services.AddTransient<IMigrationPerformers, MigrationPerformers>();
            services.AddTransient<EventStore.IEventStoreConnections, EventStore.EventStoreConnections>();
            services.AddTransient<EventStore.IMongoCollectionMigrator, EventStore.MongoCollectionMigrator>();

            AddVersionedMigrators(services);
        }
        static void AddVersionedMigrators(ServiceCollection services)
        {
            services.AddTransient<ICanMigrateDataStores, ToV7>();
            services.AddTransient<EventStore.Versions.IPerformMigrationSteps, EventStore.Versions.MigrationStepsPerformer>();
            services.AddTransient<EventStore.Versions.ToV7.Migrator>();
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
