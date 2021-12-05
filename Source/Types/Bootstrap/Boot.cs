// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Types.Bootstrap;

/// <summary>
/// Represents the entrypoint for starting up and initialization for an app using the Type system.
/// </summary>
public static class Boot
{
    /// <summary>
    /// Initialize systems needed for the type system and discovery mechanisms to work.
    /// </summary>
    /// <param name="assemblies"><see cref="IAssemblies"/> that will be used.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    /// <param name="entryAssembly"><see cref="Assembly"/> to use as entry assembly - null indicates it will ask for the entry assembly.</param>
    /// <returns><see cref="ITypeFinder"/> that can be used.</returns>
    public static ITypeFinder Start(IAssemblies assemblies, ILogger logger, Assembly entryAssembly = null)
    {
        entryAssembly ??= Assembly.GetEntryAssembly();

        var contractToImplementorsMap = new ContractToImplementorsMap();

        contractToImplementorsMap.Feed(entryAssembly.GetTypes());

        var typeFeeder = new TypeFeeder(logger);
        typeFeeder.Feed(assemblies, contractToImplementorsMap);

        return new TypeFinder(contractToImplementorsMap);
    }
}
