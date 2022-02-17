// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BaselineTypeDiscovery;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Represents a system that can scan all Runtime assemblies for classes to be registered in a DI container.
/// </summary>
public static class TypeScanner
{
    /// <summary>
    /// Scans all the Runtime assemblies to find classes and groups them by scope and lifecycle.
    /// </summary>
    /// <returns>The discovered <see cref="ClassesByScope"/>.</returns>
    public static ClassesByScope ScanRuntimeAssemblies()
    {
        var classes = GetAllClassesInRuntimeAssemblies();

        var classesByScope = classes.ToLookup(_ => _.GetScope());

        return new ClassesByScope(
            GroupClassesByLifecycle(classesByScope[Scopes.Global]),
            GroupClassesByLifecycle(classesByScope[Scopes.PerTenant]));
    }

    static IEnumerable<Type> GetAllClassesInRuntimeAssemblies()
    {
        var assemblies = AssemblyFinder.FindAssemblies(
            _ => { },
            assembly => assembly.FullName != default 
                        && assembly.FullName.StartsWith("Dolittle.Runtime", StringComparison.InvariantCulture)
                        && !assembly.FullName.Contains("Contracts", StringComparison.InvariantCulture),
            true);
        return assemblies.SelectMany(_ => _.ExportedTypes).Where(_ => _.IsClass);
    }

    static ClassesByLifecycle GroupClassesByLifecycle(IEnumerable<Type> classes)
    {
        var classesByLifecycle = classes.ToLookup(_ => _.GetLifecycle());
        
        return new ClassesByLifecycle(
            classesByLifecycle[Lifecycles.Singleton].ToArray(),
            classesByLifecycle[Lifecycles.Scoped].ToArray(),
            classesByLifecycle[Lifecycles.Transient].ToArray());
    }
}
