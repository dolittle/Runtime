// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationSource"/> for the Dolittle microservice configuration from the unified file `runtime.json`, and the legacy files in the `.dolittle` directory.
/// </summary>
public class RuntimeJsonFileConfigurationSource : IConfigurationSource
{
    /// <inheritdoc /> 
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var runtimeConfigFile = new PhysicalFileInfo(new FileInfo(Path.GetFullPath("runtime.json")));

        IFileProvider? legacyFilesProvider = null;
        if (Directory.Exists(".dolittle"))
        {
            legacyFilesProvider = new PhysicalFileProvider(Path.GetFullPath(".dolittle"));
        }

        return new RuntimeFileConfigurationProvider(runtimeConfigFile, legacyFilesProvider, (builder, file) =>
        {
            if (file.Exists)
            {
                builder.AddJsonFile(file.PhysicalPath);
            }
            return builder.Build();
        });
    }
}
