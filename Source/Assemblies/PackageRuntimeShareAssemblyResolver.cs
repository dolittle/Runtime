// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dolittle.Runtime.Versioning;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents a <see cref="ICompilationAssemblyResolver"/> that tries to resolve from the package runtime shared store.
    /// </summary>
    /// <remarks>
    /// Read more here https://natemcmaster.com/blog/2018/08/29/netcore-primitives-2/
    /// https://github.com/dotnet/corefx/issues/11639.
    /// </remarks>
    public class PackageRuntimeShareAssemblyResolver : ICompilationAssemblyResolver
    {
        static readonly VersionConverter _versionConverter = new();

        /// <inheritdoc/>
        public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
        {
            string basePath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = @"c:\Program Files\dotnet\shared";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // default location on Ubuntu 19.04
                basePath = "/usr/share/dotnet/shared";
            }
            else
            {
                // keep the macOS location as the default
                basePath = "/usr/local/share/dotnet/shared";
            }

            var found = false;

            foreach (var path in Directory.GetDirectories(basePath))
            {
                if (found) break;

                var version = _versionConverter.FromString(library.Version);
                var versionDir = Path.Combine(path, $"{version.Major}.");

                var targetDirectoryToCheck = Directory.GetDirectories(path)
                                                    .Where(_ => _.StartsWith(versionDir, StringComparison.InvariantCultureIgnoreCase))
                                                    .OrderByDescending(_ => _)
                                                    .FirstOrDefault();

                if (targetDirectoryToCheck != default)
                {
                    var assemblyFileToSearchFor = $"{library.Name}.dll";

                    var file = Directory.GetFiles(targetDirectoryToCheck)
                                .SingleOrDefault(_ => Path.GetFileName(_).Equals(assemblyFileToSearchFor, StringComparison.InvariantCultureIgnoreCase));
                    if (file != null)
                    {
                        found = true;
                        assemblies.Add(file);
                    }
                }
            }

            return found;
        }
    }
}
