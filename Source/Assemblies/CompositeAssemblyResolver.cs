// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents a <see cref="ICompilationAssemblyResolver"/> that can run through multiple resolvers.
/// </summary>
public class CompositeAssemblyResolver : ICompilationAssemblyResolver
{
    readonly ICompilationAssemblyResolver[] _resolvers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeAssemblyResolver"/> class.
    /// </summary>
    /// <param name="resolvers">Params of <see cref="ICompilationAssemblyResolver">resolvers</see>.</param>
    public CompositeAssemblyResolver(params ICompilationAssemblyResolver[] resolvers)
    {
        _resolvers = resolvers;
    }

    /// <inheritdoc/>
    public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
    {
        var found = false;

        foreach (var resolver in _resolvers)
        {
            try
            {
                found |= resolver.TryResolveAssemblyPaths(library, assemblies);
                if (assemblies.Count > 0)
                {
                    break;
                }
            }
            catch { }
        }

        return found;
    }
}
