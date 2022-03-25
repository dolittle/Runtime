// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration.Legacy;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationSource"/> for the legacy Dolittle microservice configuration files under the .dolittle folder.
/// </summary>
public class LegacyConfigurationSource : IConfigurationSource
{
    /// <inheritdoc /> 
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new LegacyConfigurationProvider(new PhysicalFileProvider(Path.GetFullPath(".dolittle")));
}
