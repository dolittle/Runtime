// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Assemblies.Configuration;
using Dolittle.Runtime.Assemblies.Rules;
using Dolittle.Runtime.Logging;

namespace Dolittle.Runtime.Assemblies.Bootstrap
{
    /// <summary>
    /// Represents the entrypoint for initializing assemblies.
    /// </summary>
    public static class Boot
    {
        /// <summary>
        /// Initialize assemblies setup.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> to use for logging.</param>
        /// <param name="entryAssembly"><see cref="Assembly"/> to use as entry assembly - null indicates it will get it from the <see cref="Assembly.GetEntryAssembly()"/> method.</param>
        /// <param name="defaultAssemblyProvider">The default <see cref="ICanProvideAssemblies"/> - null inidicates it will use the default implementation.</param>
        /// <param name="excludeAllCallback">A callback to build on the exclude all specification.</param>
        /// <returns>Discovered <see cref="IAssemblies"/>.</returns>
        public static IAssemblies Start(
            ILogger logger,
            Assembly entryAssembly = null,
            ICanProvideAssemblies defaultAssemblyProvider = null,
            Action<ExcludeAll> excludeAllCallback = null)
        {
            var assembliesConfigurationBuilder = new AssembliesConfigurationBuilder();
            var assembliesSpecification = assembliesConfigurationBuilder
                .ExcludeAll()
                .ExceptProjectLibraries()
                .ExceptDolittleLibraries();

            excludeAllCallback?.Invoke(assembliesSpecification);

            if (entryAssembly == null)
            {
                entryAssembly = Assembly.GetEntryAssembly();
            }

            var assembliesConfiguration = new AssembliesConfiguration(assembliesConfigurationBuilder.RuleBuilder);
            var assemblyFilters = new AssemblyFilters(assembliesConfiguration);

            var assemblyProvider = new AssemblyProvider(
                new ICanProvideAssemblies[] { defaultAssemblyProvider ?? new DefaultAssemblyProvider(logger, entryAssembly) },
                assemblyFilters,
                new AssemblyUtility(),
                logger);

            var assemblies = new Assemblies(entryAssembly, assemblyProvider);
            return assemblies;
        }
    }
}