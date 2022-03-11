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
    static readonly string _delimiter = ConfigurationPath.KeyDelimiter;
    static readonly string _dolittleConfigSectionRoot = $"dolittle{_delimiter}runtime";

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
            case "microservices.json":
                MapIntoRoot("microservices", config);
                break;
            case "resources.json":
                MapResources(config);
                break;
            case "event-horizon-consents.json":
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

    void MapEventHorizonConsents(IConfiguration config)
    {
        foreach (var microservicesForTenant in config.GetChildren())
        {
            var sectionPrefix = $"{_dolittleConfigSectionRoot}{_delimiter}tenants{_delimiter}{microservicesForTenant.Key}{_delimiter}eventHorizons";
            Data.Add(sectionPrefix, null);
            foreach (var consent in microservicesForTenant.GetChildren())
            {
                var eventHorizonMicroservicePrefix = $"{sectionPrefix}{_delimiter}{consent["microservice"]}"; 
                Data.Add(eventHorizonMicroservicePrefix, null);
                var consentSectionPrefix = $"{eventHorizonMicroservicePrefix}{_delimiter}consents";
                Data.Add(consentSectionPrefix, null);
                var consentForConsumerSectionPrefix = $"{consentSectionPrefix}{_delimiter}{consent["tenant"]}";
                Data.Add($"{consentForConsumerSectionPrefix}{_delimiter}stream", consent["stream"]);
                Data.Add($"{consentForConsumerSectionPrefix}{_delimiter}partition", consent["partition"]);
                Data.Add($"{consentForConsumerSectionPrefix}{_delimiter}consent", consent["consent"]);
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
}
