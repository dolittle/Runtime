// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Dolittle.Runtime.Assemblies;

/// <summary>
/// Represents an implementation of <see cref="ICompilationAssemblyResolver"/>.
/// </summary>
/// <remarks>
/// This is based on https://github.com/dotnet/core-setup/blob/master/src/managed/Microsoft.Extensions.DependencyModel/Resolution/PackageCompilationAssemblyResolver.cs.
/// We need it to actually find the path to the assembly if its not in the library, so extending the behavior.
/// </remarks>
public class PackageCompilationAssemblyResolver : ICompilationAssemblyResolver
{
    readonly string[] _nugetPackageDirectories;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageCompilationAssemblyResolver"/> class.
    /// </summary>
    public PackageCompilationAssemblyResolver()
    {
        _nugetPackageDirectories = GetDefaultProbeDirectories();
    }

    /// <inheritdoc/>
    public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
    {
        if (_nugetPackageDirectories == null || _nugetPackageDirectories.Length == 0 ||
            !string.Equals(library.Type, "package", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        foreach (var directory in _nugetPackageDirectories)
        {
            if (!TryResolvePackagePath(library, directory, out var packagePath))
            {
                continue;
            }
            if (!TryResolveFromPackagePath(library, packagePath, out var fullPathsFromPackage))
            {
                continue;
            }
            if (fullPathsFromPackage.Any())
            {
                assemblies.AddRange(fullPathsFromPackage);
            }
            else
            {
                var libPath = Path.Join(packagePath, "lib");
                var dllName = $"{library.Name}.dll";
                var paths = Directory.EnumerateFiles(libPath, dllName, SearchOption.AllDirectories);
                assemblies.AddRange(paths);
            }

            return true;
        }

        return false;
    }

    static string[] GetDefaultProbeDirectories()
    {
        var probeDirectories = AppDomain.CurrentDomain.GetData("PROBING_DIRECTORIES");
        var listOfDirectories = probeDirectories as string;

        if (!string.IsNullOrEmpty(listOfDirectories))
        {
            return listOfDirectories.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        var packageDirectory = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

        if (!string.IsNullOrEmpty(packageDirectory))
        {
            return new[] { packageDirectory };
        }

        string basePath;
        basePath = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "USERPROFILE"
            : "HOME");

        return string.IsNullOrEmpty(basePath)
            ? new[] { string.Empty }
            : new[] { Path.Combine(basePath, ".nuget", "packages") };
    }

    static bool TryResolveFromPackagePath(CompilationLibrary library, string basePath, out IEnumerable<string> results)
    {
        var paths = new List<string>();

        foreach (var assembly in library.Assemblies)
        {
            if (!TryResolveAssemblyFile(basePath, assembly, out var fullName))
            {
                results = null;
                return false;
            }

            paths.Add(fullName);
        }

        results = paths;
        return true;
    }

    static bool TryResolvePackagePath(CompilationLibrary library, string basePath, out string packagePath)
    {
        var path = library.Path;
        if (string.IsNullOrEmpty(path))
        {
            path = Path.Combine(library.Name, library.Version);
        }

        packagePath = Path.Combine(basePath, path);

        return Directory.Exists(packagePath);
    }

    static bool TryResolveAssemblyFile(string basePath, string assemblyPath, out string fullName)
    {
        fullName = Path.Combine(basePath, assemblyPath);
        return File.Exists(fullName);
    }
}
