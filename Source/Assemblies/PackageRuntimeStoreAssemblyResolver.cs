// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents a <see cref="ICompilationAssemblyResolver"/> that tries to resolve from the package runtime store.
    /// </summary>
    /// <remarks>
    /// Read more here : https://docs.microsoft.com/en-us/dotnet/core/deploying/runtime-store
    /// macOS : /usr/local/share/dotnet/store/{CPU}/{targetFramework e.g. netcoreapp2.0}/{package path}
    /// Linux : /usr/share/dotnet/store/{CPU}/{targetFramework e.g. netcoreapp2.0}/{package path}
    /// Windows : C:/Program Files/dotnet/store/{CPU}/{targetFramework e.g. netcoreapp2.0}/{package path}.
    /// </remarks>
    public class PackageRuntimeStoreAssemblyResolver : ICompilationAssemblyResolver
    {
        /// <inheritdoc/>
        public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
        {
            string basePath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = @"c:\Program Files\dotnet\store";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // couldn't find this on Ubuntu 19.04 on a fresh dotnet sdk2.2 install
                basePath = "/usr/share/dotnet/store";
            }
            else
            {
                // keep the macOS location as the default
                basePath = "/usr/local/share/dotnet/store";
            }

#pragma warning disable CA1308
            var cpuBasePath = Path.Combine(basePath, RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant());
#pragma warning restore CA1308
            if (!Directory.Exists(cpuBasePath)) return false;

            var found = false;

            foreach (var targetFrameworkBasePath in Directory.GetDirectories(cpuBasePath))
            {
                var libraryBasePath = Path.Combine(targetFrameworkBasePath, library.Path);
                foreach (var assembly in library.Assemblies)
                {
                    var assemblyPath = Path.Combine(libraryBasePath, assembly);
                    if (File.Exists(assemblyPath))
                    {
                        assemblies.Add(assemblyPath);
                        found = true;
                    }
                }
            }

            return found;
        }
    }
}
