// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents a <see cref="ICanProvideAssemblies">assembly provider</see> that will provide only well known assemblies.
/// </summary>
public class WellKnownAssembliesAssemblyProvider : ICanProvideAssemblies
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WellKnownAssembliesAssemblyProvider"/> class.
    /// </summary>
    /// <param name="assemblies"><see cref="IEnumerable{T}">Collection</see> of <see cref="AssemblyName"/> with all known assemblies.</param>
    public WellKnownAssembliesAssemblyProvider(IEnumerable<AssemblyName> assemblies)
    {
        Libraries = assemblies.Select(_ =>
            new Library("Package", _.Name, _.Version.ToString(), string.Empty, Array.Empty<Dependency>(), false));
    }

    /// <inheritdoc/>
    public IEnumerable<Library> Libraries { get; }

    /// <inheritdoc/>
    public Assembly GetFrom(Library library)
    {
        return Assembly.Load(library.Name);
    }
}