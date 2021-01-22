// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Booting.Stages;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="DiscoverySettings"/>.
    /// </summary>
    public static class DiscoveryBootBuilderExtensions
    {
        /// <summary>
        /// With a set of known <see cref="AssemblyName">assemblies</see>.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="assemblies"><see cref="IEnumerable{T}"/> of <see cref="AssemblyName"/> to use as well known assemblies, instead of discovering.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder WithAssemblies(this IBootBuilder bootBuilder, IEnumerable<AssemblyName> assemblies)
        {
            bootBuilder.Set<DiscoverySettings>(_ => _.AssemblyProvider, new WellKnownAssembliesAssemblyProvider(assemblies));
            return bootBuilder;
        }

        /// <summary>
        /// With a custom <see cref="ICanProvideAssemblies"/>.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="assemblyProvider">An <see cref="ICanProvideAssemblies">assembly provider</see> instance.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder WithAssemblyProvider(this IBootBuilder bootBuilder, ICanProvideAssemblies assemblyProvider)
        {
            bootBuilder.Set<DiscoverySettings>(_ => _.AssemblyProvider, assemblyProvider);
            return bootBuilder;
        }

        /// <summary>
        /// Include assemblies that start with a certain name in the discovery.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="names">Params of names to include.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder IncludeAssembliesStartingWith(this IBootBuilder bootBuilder, params string[] names)
        {
            bootBuilder.Set<DiscoverySettings>(_ => _.IncludeAssembliesStartWith, names);
            return bootBuilder;
        }
    }
}