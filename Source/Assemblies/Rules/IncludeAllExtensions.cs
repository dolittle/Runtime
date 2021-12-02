// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Specifications;

namespace Dolittle.Runtime.Assemblies.Rules;

/// <summary>
/// Extensions for <see cref="IncludeAll"/>.
/// </summary>
public static class IncludeAllExtensions
{
    /// <summary>
    /// Include all except for assemblies that has a name starting with the given name.
    /// </summary>
    /// <param name="includeAll"><see cref="IncludeAll">Configuration object</see>.</param>
    /// <param name="assemblyNames">Names of assemblies to exclude.</param>
    /// <returns>Chain of <see cref="IncludeAll">configuration object</see>.</returns>
    public static IncludeAll ExceptAssembliesStartingWith(this IncludeAll includeAll, params string[] assemblyNames)
    {
        var specification = includeAll.Specification;

        foreach (var assemblyName in assemblyNames)
            specification = specification.And(new ExceptAssembliesStartingWith(assemblyName));

        includeAll.Specification = specification;
        return includeAll;
    }
}