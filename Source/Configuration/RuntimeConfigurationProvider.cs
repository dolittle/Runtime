// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="ConfigurationProvider"/> that provides Dolittle configurations
/// from the legacy .dolittle folder configuration files.
/// </summary>
public class RuntimeConfigurationProvider : JsonConfigurationProvider
{
    static readonly string _delimiter = ConfigurationPath.KeyDelimiter;
    static readonly string _dolittleConfigSectionRoot = $"dolittle{_delimiter}runtime";

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeConfigurationProvider"/> class.
    /// </summary>
    public RuntimeConfigurationProvider(JsonConfigurationSource source)
        : base(source)
    {
    }


    /// <inheritdoc />
    public override void Load()
    {
        base.Load();
        foreach (var (key, value) in Data.ToArray())
        {
            var newKey = $"{_dolittleConfigSectionRoot}{_delimiter}{key}";
            Data.Remove(key);
            Data.Add(newKey, value);
        }
    }

}
