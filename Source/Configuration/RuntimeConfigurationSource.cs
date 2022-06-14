// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IConfigurationSource"/> for the unified Dolittle microservice configuration file called `runtime.json`.
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
            ReloadOnChange = true
        };
        source.EnsureDefaults(builder);
        source.ResolveFileProvider();
        return new RuntimeConfigurationProvider(source);
    }
}
