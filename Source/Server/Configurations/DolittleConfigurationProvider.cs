// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Dolittle.Runtime.Server.Configurations;

public class DolittleConfigurationProvider : ConfigurationProvider
{
    static readonly Dictionary<string, string> _files = new()
    {
        ["endpoints.json"] = $"dolittle{ConfigurationPath.KeyDelimiter}endpoints",
    };

    
    public DolittleConfigurationProvider()
    {
        
        foreach (var file in _files)
        {
            
        }
    }
    
    public override void Load()
    {
        foreach (var file in _files)
        {
            var provider = new JsonConfigurationProvider(new JsonConfigurationSource
            {
                Path = Path.GetFullPath(Path.Combine(".dolittle", file.Key))
            });
            provider.Load();
            var keys = provider.GetChildKeys(Enumerable.Empty<string>(), null);
            foreach (var key in keys)
            {
                if (provider.TryGet(key, out var value))
                {
                    Data[$"{file.Value}{ConfigurationPath.KeyDelimiter}{key}"] = value;
                }
            }
        }
    }
}
