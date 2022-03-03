// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.CLI.Configuration.Files;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.CLI.Configuration.Runtime;

/// <summary>
/// Represents an implementation of <see cref="IRuntimeConfiguration"/>
/// </summary>
public class RuntimeConfiguration : IRuntimeConfiguration
{
    readonly IFileProvider _files;
    readonly ISerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeConfiguration"/> class.
    /// </summary>
    /// <param name="files">The file provider to use to locate files in the filesystem.</param>
    /// <param name="serializer">The file serializer to use to parse the contents of the configuration files.</param>
    public RuntimeConfiguration(IFileProvider files, ISerializer serializer)
    {
        _files = files;
        _serializer = serializer;
    }

    /// <inheritdoc />
    public ResourceConfigurationsByTenant Resources
    {
        get
        {
            var file = _files.GetFileInfo(".dolittle/resources.json");
            return _serializer.FromJsonFile<ResourceConfigurationsByTenant>(file);
        }
    }
}
