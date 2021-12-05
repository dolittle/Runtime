// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents a <see cref="ICompilationAssemblyResolver"/> for the NuGet fallback folder.
/// </summary>
/// <remarks>
/// macOS : /usr/local/share/dotnet/sdk/NuGetFallbackFolder/{package path}
/// Linux : /usr/share/dotnet/sdk/NuGetFallbackFolder/{package path}
/// Windows : C:/Program Files/dotnet/sdk/NuGetFallbackFolder/{package path}.
/// </remarks>
public class NuGetFallbackFolderAssemblyResolver : ICompilationAssemblyResolver
{
    /// <inheritdoc/>
    public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
    {
        string basePath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            basePath = @"c:\Program Files\dotnet\sdk\NuGetFallbackFolder";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // default location on Ubuntu 19.04
            basePath = "/usr/share/dotnet/sdk/NuGetFallbackFolder";
        }
        else
        {
            // keep the macOS location as the default
            basePath = "/usr/local/share/dotnet/sdk/NuGetFallbackFolder";
        }

        if (!Directory.Exists(basePath))
        {
            return false;
        }

        var found = false;

        var libraryBasePath = Path.Combine(basePath, library.Path);
        foreach (var assembly in library.Assemblies)
        {
            var assemblyPath = Path.Combine(libraryBasePath, assembly);
            if (!File.Exists(assemblyPath))
            {
                continue;
            }
            assemblies.Add(assemblyPath);
            found = true;
        }

        return found;
    }
}
