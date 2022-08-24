// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationSource"/> for the Dolittle microservice configuration from the unified file `runtime.json`, and the legacy files in the `.dolittle` directory.
/// </summary>
public class RuntimeFileConfigurationSource : IConfigurationSource
{
    /// <inheritdoc /> 
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new RuntimeFileConfigurationProvider(new PhysicalFileProvider(Path.GetFullPath(".dolittle")));
}
