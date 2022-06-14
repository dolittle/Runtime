// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="ConfigurationProvider"/> that provides Dolittle configurations
/// from a unified configuration json file.
/// </summary>
public class RuntimeConfigurationProvider : JsonConfigurationProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeConfigurationProvider"/> class.
    /// </summary>
    public RuntimeConfigurationProvider(JsonConfigurationSource source)
        : base(source)
    { }

    /// <inheritdoc />
    public override void Load(Stream stream)
    {
        base.Load(stream);
        AddPrefixToDataKeys();
    }

    void AddPrefixToDataKeys()
    {
        foreach (var (key, value) in Data.ToArray())
        {
            var newKey = Constants.CombineWithDolittleConfigRoot(key);
            Data.Remove(key);
            Data.Add(newKey, value);
        }
    }
}
