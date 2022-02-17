// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration.Legacy;

public class LegacyConfigurationProvider : ConfigurationProvider
{
    readonly IFileProvider _fileProvider;

    public LegacyConfigurationProvider(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public override void Load()
    {
        foreach (var file in _fileProvider.GetDirectoryContents("/"))
        {
            var fileBuilder = new ConfigurationBuilder();
            fileBuilder.AddJsonFile(_fileProvider, file.Name, false, false);
            var fileConfiguration = fileBuilder.Build();
            MapConfiguration(file.Name, fileConfiguration);
        }
    }

    void MapConfiguration(string file, IConfiguration configuration)
    {
        switch (file)
        {
            case "endpoints.json":
                MapEndpoints(configuration);
                break;
            case "tenants.json":
                MapTenants(configuration);
                break;
            case "resources.json":
                MapResources(configuration);
                break;
        }
    }

    void MapEndpoints(IConfiguration endpoints)
    {
        const string rootPath = "dolittle:runtime:endpoints";
        foreach (var kvp in GetData(rootPath, endpoints))
        {
            Data.Add(kvp);
        }
        
    }
    void MapTenants(IConfiguration tenants)
    {
        const string rootPath = "dolittle:runtime:tenants";
        foreach (var kvp in GetData(rootPath, tenants))
        {
            Data.Add(kvp);
        }
    }
    void MapResources(IConfiguration resources)
    {
        const string rootPath = "dolittle:runtime:resources";
        foreach (var kvp in GetData(rootPath, resources))
        {
            Data.Add(kvp);
        }
    }

    static IEnumerable<KeyValuePair<string, string>> GetData(string rootPath, IConfiguration config)
        => config.AsEnumerable()
            .Where(keyAndValue => keyAndValue.Value != null || !config.GetSection(keyAndValue.Key).GetChildren().Any())
            .Select(keyAndValue => new KeyValuePair<string,string>($"{rootPath}:{keyAndValue.Key}", keyAndValue.Value));
}
