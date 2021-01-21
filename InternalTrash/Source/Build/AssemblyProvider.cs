// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Logging;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents a <see cref="ICanProvideAssemblies"/> for providing all the default assemblies plus plugins.
    /// </summary>
    public class AssemblyProvider : ICanProvideAssemblies
    {
        static readonly string[] _excludedAssemblies = new string[] { "Microsoft", "System", "Dolittle.Runtime" };
        readonly DefaultAssemblyProvider _innerProvider;
        readonly IEnumerable<Library> _baseLibraries;
        readonly List<Library> _localLibraries = new List<Library>();
        readonly List<Library> _libraries = new List<Library>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyProvider"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        /// <param name="pluginAssemblies"><see cref="IEnumerable{T}"/> of string representations of assemblies containing plugins.</param>
        public AssemblyProvider(ILogger logger, IEnumerable<string> pluginAssemblies)
        {
            _innerProvider = new DefaultAssemblyProvider(logger);
            _baseLibraries = _innerProvider.Libraries;
            _libraries.AddRange(_baseLibraries);

            pluginAssemblies.ForEach(pluginAssembly =>
            {
                var assembly = Assembly.LoadFrom(pluginAssembly);
                var basePath = Path.GetDirectoryName(assembly.Location);
                var assemblyName = assembly.GetName();
                _localLibraries.Add(CreateLibraryFrom(basePath, assemblyName));

                var assemblies = assembly.GetReferencedAssemblies();
                var libraries = assemblies
                                    .Where(_ => !_baseLibraries.Any(l => l.Name == _.Name))
                                    .Select(_ => CreateLibraryFrom(basePath, _))
                                    .Where(_ => _ != null)
                                    .ToArray();
                _localLibraries.AddRange(libraries);
            });

            _libraries.AddRange(_localLibraries);
        }

        /// <inheritdoc/>
        public IEnumerable<Library> Libraries => _libraries;

        /// <inheritdoc/>
        public Assembly GetFrom(Library library)
        {
            if (_baseLibraries.Contains(library)) return _innerProvider.GetFrom(library);
            var assembly = Assembly.LoadFrom(library.Path);
            return assembly;
        }

        static string GetPathForAssembly(string basePath, AssemblyName assemblyName)
        {
            var path = Path.Combine(basePath, assemblyName.Name);
            var dllPath = $"{path}.dll";
            var exePath = $"{path}.exe";
            if (File.Exists(dllPath)) return dllPath;
            else if (File.Exists(exePath)) return exePath;
            return string.Empty;
        }

        Library CreateLibraryFrom(string basePath, AssemblyName assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName.Name) ||
                assemblyName.Version == null ||
                _excludedAssemblies.Any(_ => assemblyName.Name.StartsWith(_, StringComparison.InvariantCulture)))
            {
                return null;
            }

            var path = GetPathForAssembly(basePath, assemblyName);
            if (string.IsNullOrEmpty(path)) return null;

            return new Library(
                "Project",
                assemblyName.Name,
                assemblyName.Version.ToString(),
                string.Empty,
                Array.Empty<Dependency>(),
                false,
                path,
                string.Empty);
        }
    }
}