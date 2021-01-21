// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.Loader;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Booting;
using Dolittle.Logging.Internal;

namespace Dolittle.Build.CLI
{
    /// <summary>
    /// The entrypoint of the CLI.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Holds The <see cref="BuildTarget"/>.
        /// </summary>
        internal static BuildTarget BuildTarget;

        static int Main(string[] args)
        {
            try
            {
                var startTime = DateTime.UtcNow;

                var assemblyFile = args[0];
                var pluginAssemblies = args[1].Split(";");
                var configurationFile = args[2];
                var outputAssemblyFile = args[3];

                if (string.IsNullOrEmpty(args[1]) ||
                    pluginAssemblies.Length == 0 ||
                    string.IsNullOrEmpty(configurationFile))
                {
                    return 0;
                }

                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);
                var assemblyContext = AssemblyContext.From(assemblyFile);
                BuildTarget = new BuildTarget(assemblyFile, outputAssemblyFile, assembly, assemblyContext);

                Console.WriteLine("Performing Dolittle post-build steps");

                Console.WriteLine($"  Performing for: {assemblyFile}");
                Console.WriteLine($"  Will output to: {outputAssemblyFile}");
                Console.WriteLine("  Using plugins from: ");

                foreach (var pluginAssembly in pluginAssemblies)
                    Console.WriteLine($"    {pluginAssembly}");

                var bootLoaderResult = Bootloader.Configure(_ => _
                    .WithAssemblyProvider(new AssemblyProvider(NullLoggerManager.Instance.CreateLogger<AssemblyProvider>(), pluginAssemblies))
                    .NoLogging()
                    .SkipBootprocedures()).Start();

                var buildMessages = bootLoaderResult.Container.Get<IBuildMessages>();

                var configuration = bootLoaderResult.Container.Get<IPerformerConfigurationManager>();
                configuration.Initialize(configurationFile);
                var buildTaskPerformers = bootLoaderResult.Container.Get<IBuildTaskPerformers>();
                buildTaskPerformers.Perform();

                var assemblyModifiers = bootLoaderResult.Container.Get<ITargetAssemblyModifiers>();
                assemblyModifiers.ModifyAndSave();

                var postTasksPerformers = bootLoaderResult.Container.Get<IPostBuildTaskPerformers>();
                postTasksPerformers.Perform();

                var endTime = DateTime.UtcNow;
                var deltaTime = endTime.Subtract(startTime);
                buildMessages.Information($"Time Elapsed {deltaTime.ToString("G", CultureInfo.InvariantCulture)} (Dolittle)");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error executing Dolittle post build tool");
                Console.Error.WriteLine($"Exception: {ex.Message}");
                Console.Error.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.ResetColor();
                return 1;
            }

            return 0;
        }
    }
}