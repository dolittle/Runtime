// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Assemblies;

namespace Dolittle.Runtime.Configuration.Files;

/// <summary>
/// Represents a <see cref="ICanProvideConfigurationObjects">configuration provider</see> for
/// embedded resources.
/// </summary>
/// <remarks>
/// It will only look for embedded resources in the entry assembly.
/// </remarks>
public class ManifestResourceStreamConfigurationObjectsProvider : ICanProvideConfigurationObjects
{
    readonly Assembly _entryAssembly;
    readonly IConfigurationFileParsers _parsers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManifestResourceStreamConfigurationObjectsProvider"/> class.
    /// </summary>
    /// <param name="assemblies"><see cref="IAssemblies"/> for getting assembly for embedded resources.</param>
    /// <param name="parsers"><see cref="IConfigurationFileParsers"/> for parsing.</param>
    public ManifestResourceStreamConfigurationObjectsProvider(
        IAssemblies assemblies,
        IConfigurationFileParsers parsers)
    {
        _entryAssembly = assemblies.EntryAssembly;
        _parsers = parsers;
    }

    /// <inheritdoc/>
    public bool CanProvide(Type type)
    {
        var filename = GetFilenameFor(type);
        var resourceNames = _entryAssembly.GetManifestResourceNames().Where(_ => _.EndsWith(filename, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        if (resourceNames.Length > 1) throw new MultipleFilesAvailableOfSameType(type, resourceNames);
        return resourceNames.Length == 1;
    }

    /// <inheritdoc/>
    public object Provide(Type type)
    {
        var filename = GetFilenameFor(type);
        var resourceName = _entryAssembly.GetManifestResourceNames().SingleOrDefault(_ => _.EndsWith(filename, StringComparison.InvariantCultureIgnoreCase));
        if (resourceName != null)
        {
            using var reader = new StreamReader(_entryAssembly.GetManifestResourceStream(resourceName));
            var content = reader.ReadToEnd();
            return _parsers.Parse(type, resourceName, content);
        }

        throw new UnableToProvideConfigurationObject<ManifestResourceStreamConfigurationObjectsProvider>(type);
    }

    string GetFilenameFor(Type type)
    {
        return $"{type.GetFriendlyConfigurationName()}.json";
    }
}