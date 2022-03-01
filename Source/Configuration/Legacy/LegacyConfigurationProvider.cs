// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration.Legacy;

/// <summary>
/// Represents an implementation of <see cref="ConfigurationProvider"/> that provides Dolittle configurations
/// from the legacy .dolittle folder configuration files.
/// </summary>
public class LegacyConfigurationProvider : ConfigurationProvider
{
    static string _delimiter = ConfigurationPath.KeyDelimiter;
    static string _dolittleConfigSectionRoot = $"dolittle{_delimiter}runtime";

    readonly IFileProvider _fileProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyConfigurationProvider"/> class.
    /// </summary>
    /// <param name="fileProvider"></param>
    public LegacyConfigurationProvider(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }


    /// <inheritdoc />
    public override void Load()
    {
        foreach (var file in _fileProvider.GetDirectoryContents("/"))
        {
            var fileBuilder = new ConfigurationBuilder();
            fileBuilder.AddJsonFile(_fileProvider, file.Name, false, false);
            var configuration = fileBuilder.Build();
            MapDolittleFile(file.Name, configuration); 
        }
    }

    void MapDolittleFile(string file, IConfiguration config)
    {
        switch (file)
        {
            case "endpoints.json":
                MapIntoRoot("endpoints", config);
                break;
            case "metrics.json":
                MapIntoRoot($"endpoints{_delimiter}metrics", config);
                break;
            case "platform.json":
                MapIntoRoot("platform", config);
                break;
            case "tenants.json":
                MapIntoRoot("tenants", config);
                break;
            case "resources.json":
                MapResources(config);
                break;
            case "microservices.json":
                MapMicroservices(config);
                break;
            case "event-horizon-consents":
                MapEventHorizonConsents(config);
                break;
        }
    }

    void MapIntoRoot(string sectionRoot, IConfiguration config)
    {
        foreach (var kvp in GetData($"{_dolittleConfigSectionRoot}{_delimiter}{sectionRoot}", config))
        {
            Data.Add(kvp);
        }
    }
    
    void MapResources(IConfiguration config)
    {
        foreach (var resourceForTenant in config.GetChildren().Select(_ => config.GetSection(_.Key)))
        {
            foreach (var kvp in GetData(
                         $"{_dolittleConfigSectionRoot}{_delimiter}tenants{_delimiter}{resourceForTenant.Key}{_delimiter}resources",
                         resourceForTenant))
            {
                Data.Add(kvp);
            }
        }
    }

    void MapMicroservices(IConfiguration config)
    {
        foreach (var microservicesForTenant in config.GetChildren())
        {
            foreach (var kvp in GetData(
                         $"{_dolittleConfigSectionRoot}{_delimiter}tenants{_delimiter}{microservicesForTenant.Key}{_delimiter}eventHorizons",
                         microservicesForTenant))
            {
                Data.Add(kvp);
            }
        }
    }
    void MapEventHorizonConsents(IConfiguration config)
    {
        foreach (var microservicesForTenant in config.GetChildren())
        {
            foreach (var kvp in GetData(
                         $"{_dolittleConfigSectionRoot}{_delimiter}{microservicesForTenant.Key}{_delimiter}eventHorizons",
                         microservicesForTenant))
            {
                Data.Add(kvp);
            }
        }
    }

    static IEnumerable<KeyValuePair<string, string>> GetData(string rootPath, IConfiguration config)
    {
        var data = new List<KeyValuePair<string, string>>();
        foreach (var section in config.GetChildren())
        {
            data.Add(new KeyValuePair<string, string>($"{rootPath}{_delimiter}{section.Key}", section.Value));
            foreach (var subSection in section.GetChildren())
            {
                
                data.Add(new KeyValuePair<string, string>($"{rootPath}{_delimiter}{section.Key}{_delimiter}{subSection.Key}", subSection.Value));
                data.AddRange(GetData($"{rootPath}{_delimiter}{section.Key}{_delimiter}{subSection.Key}", subSection));
            }
        }
        return data;
    }    
    // => config.AsEnumerable()
        //     .Select(_ => config.GetSection(_.Key))
        //     .Where(_ => _.Value != null || !_.GetChildren().Any())
        //     .Select(_ => new KeyValuePair<string,string>($"{rootPath}{_delimiter}{_.Key}", _.Value));
}
