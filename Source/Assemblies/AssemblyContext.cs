// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents an implementation of <see cref="IAssemblyContext"/>.
    /// </summary>
    public class AssemblyContext : IAssemblyContext
    {
        readonly ICompilationAssemblyResolver _assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyContext"/> class.
        /// </summary>
        /// <param name="assembly">Assembly the context is for.</param>
        public AssemblyContext(Assembly assembly)
        {
            Assembly = assembly;

            AssemblyLoadContext.Default.Resolving += OnResolving;

            DependencyContext = DependencyContext.Load(assembly);

            var codeBaseUri = new Uri(assembly.Location);
            var basePath = Path.GetDirectoryName(codeBaseUri.LocalPath);

            _assemblyResolver = new CompositeAssemblyResolver(new ICompilationAssemblyResolver[]
            {
                new PackageRuntimeShareAssemblyResolver(),
                new AppBaseCompilationAssemblyResolver(basePath),
                new PackageCompilationAssemblyResolver(),
                new ReferenceAssemblyPathResolver(),
                new NuGetFallbackFolderAssemblyResolver(),
                new PackageRuntimeStoreAssemblyResolver()
            });
            AssemblyLoadContext = AssemblyLoadContext.GetLoadContext(assembly);
            AssemblyLoadContext.Resolving += OnResolving;
        }

        /// <inheritdoc/>
        public Assembly Assembly { get; }

        /// <inheritdoc/>
        public DependencyContext DependencyContext { get; }

        /// <inheritdoc/>
        public AssemblyLoadContext AssemblyLoadContext { get; }

        /// <summary>
        /// Create an <see cref="IAssemblyContext"/> from a given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> to use.</param>
        /// <returns><see cref="IAssemblyContext"/> for the <see cref="Assembly"/>.</returns>
        public static IAssemblyContext From(Assembly assembly)
        {
            return new AssemblyContext(assembly);
        }

        /// <summary>
        /// Create an <see cref="IAssemblyContext"/> from a given path to an <see cref="Assembly"/>.
        /// </summary>
        /// <param name="path">Path to the <see cref="Assembly"/> to use.</param>
        /// <returns><see cref="IAssemblyContext"/> for the path to the <see cref="Assembly"/>.</returns>
        public static IAssemblyContext From(string path)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            return From(assembly);
        }

        /// <inheritdoc/>
        public IEnumerable<Library> GetProjectReferencedLibraries()
        {
            return GetReferencedLibraries().Where(_ => _.Type.Equals("project", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public IEnumerable<Library> GetReferencedLibraries()
        {
            return DependencyContext.RuntimeLibraries.Cast<RuntimeLibrary>()
                .Where(_ => _.RuntimeAssemblyGroups.Count > 0 && !_.Name.StartsWith("runtime", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public IEnumerable<Assembly> GetProjectReferencedAssemblies()
        {
            var libraries = GetReferencedLibraries().Where(_ => _.Type.Equals("project", StringComparison.InvariantCultureIgnoreCase));
            return LoadAssembliesFrom(libraries);
        }

        /// <inheritdoc/>
        public IEnumerable<Assembly> GetReferencedAssemblies()
        {
            var libraries = GetReferencedLibraries();
            return LoadAssembliesFrom(libraries);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetAssemblyPathsFor(Library library)
        {
            var compilationLibrary = library as CompilationLibrary;
            var libraryPaths = new List<string>();
            if (compilationLibrary == null && library is RuntimeLibrary)
            {
                compilationLibrary = GetCompilationLibraryFrom(library as RuntimeLibrary);
                _assemblyResolver.TryResolveAssemblyPaths(compilationLibrary, libraryPaths);
            }

            return libraryPaths;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            AssemblyLoadContext.Resolving -= OnResolving;
            GC.SuppressFinalize(this);
        }

        Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            var compilationLibrary = GetCompilationLibraryFrom(name);
            var assemblies = new List<string>();

            _assemblyResolver.TryResolveAssemblyPaths(compilationLibrary, assemblies);
            if (assemblies.Count > 0)
            {
                try
                {
                    var assembly = assemblies[0];
                    var segments = assembly.Split(Path.DirectorySeparatorChar);
                    var hasRef = segments.Any(_ => _.Equals("ref", StringComparison.InvariantCultureIgnoreCase));
                    if (hasRef)
                    {
                        var libAssembly = assembly.Replace($"ref{Path.DirectorySeparatorChar}", $"lib{Path.DirectorySeparatorChar}", StringComparison.InvariantCulture);
                        if (File.Exists(libAssembly))
                        {
                            assembly = libAssembly;
                        }
                    }

                    return AssemblyLoadContext.LoadFromAssemblyPath(assembly);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        CompilationLibrary GetCompilationLibraryFrom(AssemblyName name)
        {
            bool NamesMatch(Library runtime)
            {
                return string.Equals(runtime.Name, name.Name, StringComparison.OrdinalIgnoreCase);
            }

            var runtimeLibrary = DependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (runtimeLibrary != null) return GetCompilationLibraryFrom(runtimeLibrary);

            return DependencyContext.CompileLibraries.FirstOrDefault(NamesMatch);
        }

        IEnumerable<Assembly> LoadAssembliesFrom(IEnumerable<Library> libraries)
        {
            return libraries
                .Select(_ =>
                {
                    try
                    {
                        return Assembly.Load(_.Name);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(_ => _ != null)
                .ToArray();
        }

        CompilationLibrary GetCompilationLibraryFrom(RuntimeLibrary library)
        {
            return new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable,
                library.Path ?? library.RuntimeAssemblyGroups.Select(g => g.AssetPaths.Count > 0 ? g.AssetPaths[0] : null).FirstOrDefault(),
                library.HashPath);
        }
    }
}
