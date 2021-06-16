// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents a system that holds information about an assembly's context.
    /// </summary>
    public interface IAssemblyContext : IDisposable
    {
        /// <summary>
        /// Gets the loaded root assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the <see cref="DependencyContext"/> for the <see cref="Assembly"/>.
        /// </summary>
        DependencyContext DependencyContext { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyLoadContext"/> for the <see cref="Assembly"/>.
        /// </summary>
        AssemblyLoadContext AssemblyLoadContext { get; }

        /// <summary>
        /// Get all the assembly paths as absolute path of a <see cref="Library"/>.
        /// </summary>
        /// <param name="library"><see cref="Library"/> to resolve.</param>
        /// <returns>Absolute path to the <see cref="Library"/>.</returns>
        IEnumerable<string> GetAssemblyPathsFor(Library library);

        /// <summary>
        /// Get all libraries that are referenced in the context.
        /// </summary>
        /// <returns>All referenced <see cref="IEnumerable{Library}">libraries</see>.</returns>
        IEnumerable<Library> GetReferencedLibraries();

        /// <summary>
        /// Get all libraries that are referenced as project in the context.
        /// </summary>
        /// <returns>All project referenced <see cref="IEnumerable{Library}">libraries</see>.</returns>
        IEnumerable<Library> GetProjectReferencedLibraries();

        /// <summary>
        /// Get all assemblies that are referenced by the assembly.
        /// </summary>
        /// <returns>All references <see cref="IEnumerable{Assembly}">assemblies</see>.</returns>
        IEnumerable<Assembly> GetReferencedAssemblies();

        /// <summary>
        /// Get assemblies that are referenced as project referenced by the assembly.
        /// </summary>
        /// <returns>Project <see cref="IEnumerable{Assembly}">assemblies</see>.</returns>
        IEnumerable<Assembly> GetProjectReferencedAssemblies();
    }
}