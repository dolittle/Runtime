// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents a <see cref="IAssemblies"/>.
/// </summary>
[Singleton]
public class Assemblies : IAssemblies
{
    readonly IEnumerable<Assembly> _assemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assemblies"/> class.
    /// </summary>
    /// <param name="entryAssembly">The entry <see cref="Assembly"/>.</param>
    /// <param name="assemblyProvider"><see cref="IAssemblyProvider"/> for providing assemblies.</param>
    public Assemblies(Assembly entryAssembly, IAssemblyProvider assemblyProvider)
    {
        EntryAssembly = entryAssembly;
        _assemblies = assemblyProvider.GetAll();
    }

    /// <inheritdoc/>
    public Assembly EntryAssembly { get; }

    /// <inheritdoc/>
    public IEnumerable<Assembly> GetAll()
    {
        return _assemblies;
    }

    /// <inheritdoc/>
    public Assembly GetByFullName(string fullName)
    {
        var query = from a in _assemblies
                    where a.FullName == fullName
                    select a;

        var assembly = query.SingleOrDefault();
        return assembly;
    }

    /// <inheritdoc/>
    public Assembly GetByName(string name)
    {
        var query = from a in _assemblies
                    where a.FullName.Contains(name, StringComparison.InvariantCulture)
                    select a;

        return query.SingleOrDefault();
    }
}