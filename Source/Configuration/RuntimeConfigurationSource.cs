// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationSource"/> for the legacy Dolittle microservice configuration files under the .dolittle folder.
/// </summary>
public class RuntimeConfigurationSource : IConfigurationSource
{
    /// <inheritdoc /> 
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var source = new JsonConfigurationSource
        {
            Optional = true,
            Path = "runtime.json",
        };
        source.EnsureDefaults(builder);
        source.ResolveFileProvider();
        return new RuntimeConfigurationProvider(source);
    }
}
