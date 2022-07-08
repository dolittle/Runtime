// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="ConfigurationProvider"/> that provides Dolittle configurations
/// from both the unified JSON file, with overrides from the legacy JSON files in the `.dolittle` directory.
/// </summary>
public class RuntimeFileConfigurationProvider : ConfigurationProvider
{
    readonly IFileProvider _dolittleConfigurationFilesProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeFileConfigurationProvider"/> class.
    /// </summary>
    /// <param name="dolittleConfigurationFilesProvider"></param>
    public RuntimeFileConfigurationProvider(IFileProvider dolittleConfigurationFilesProvider)
    {
        _dolittleConfigurationFilesProvider = dolittleConfigurationFilesProvider;
    }

    /// <inheritdoc />
    public override void Load()
    {
        var files = _dolittleConfigurationFilesProvider.GetDirectoryContents("/");
        var runtimeConfigFile = files.SingleOrDefault(file => Path.GetFileNameWithoutExtension(file.Name) == "runtime");
        if (runtimeConfigFile is not null)
        {
            MapRuntimeConfigFile(runtimeConfigFile);
        }
        MapLegacyDolittleFiles(files);
    }

    static IConfigurationRoot BuildConfigFromFile(IFileInfo file)
    {
        var builder = new ConfigurationBuilder();
        if (file.Exists)
        {
            builder.AddYamlFile(file.PhysicalPath);
        }
        return builder.Build();
    }

    void MapRuntimeConfigFile(IFileInfo file)
    {
        SetAllFromMap(GetData(Constants.DolittleConfigSectionRoot, BuildConfigFromFile(file)));
    }
    
    void MapLegacyDolittleFiles(IEnumerable<IFileInfo> files)
    {
        foreach (var file in files)
        {
            MapLegacyDolittleFile(Path.GetFileNameWithoutExtension(file.Name), BuildConfigFromFile(file));
        }
    }
    void MapLegacyDolittleFile(string fileNameWithoutExtension, IConfiguration config)
    {
        switch (fileNameWithoutExtension)
        {
            case "endpoints":
                MapIntoRoot("endpoints", config);
                break;
            case "metrics":
                MapIntoRoot(ConfigurationPath.Combine("endpoints", "metrics"), config);
                break;
            case "platform":
                MapIntoRoot("platform", config);
                break;
            case "microservices":
                MapIntoRoot("microservices", config);
                break;
            case "resources":
                MapResources(config);
                break;
            case "event-horizon-consents":
                MapEventHorizonConsents(config);
                break;
        }
    }

    void SetAllFromMap(IEnumerable<KeyValuePair<string, string>> map)
    {
        foreach (var (key, value) in map)
        {
            Data[key] = value;
        }
    }
    
    void MapIntoRoot(string sectionRoot, IConfiguration config)
    {
        SetAllFromMap(GetData(Constants.CombineWithDolittleConfigRoot(sectionRoot), config));
    }
    
    void MapResources(IConfiguration config)
    {
        foreach (var key in Data.Keys.Where(_ => _.StartsWith(Constants.CombineWithDolittleConfigRoot("tenants"), StringComparison.InvariantCulture)).ToArray())
        {
            Data.Remove(key);
        }
        foreach (var resourceForTenant in config.GetChildren().Select(_ => config.GetSection(_.Key)))
        {
            SetAllFromMap(GetData(
                Constants.CombineWithDolittleConfigRoot("tenants", resourceForTenant.Key, "resources"),
                resourceForTenant));
        }
    }

    void MapEventHorizonConsents(IConfiguration config)
    {
        foreach (var microservicesForTenant in config.GetChildren())
        {
            var sectionPrefix = Constants.CombineWithDolittleConfigRoot("tenants", microservicesForTenant.Key, "eventHorizons");
            foreach (var consentsPerConsumerMicroservice in microservicesForTenant.GetChildren().GroupBy(_ => _["microservice"]))
            {
                var consumerMicroservice = consentsPerConsumerMicroservice.Key;
                var eventHorizonMicroservicePrefix = ConfigurationPath.Combine(sectionPrefix, consumerMicroservice);
                var consentSectionPrefix = ConfigurationPath.Combine(eventHorizonMicroservicePrefix, "consents");
                var consentsPerConsumerMicroserviceArray = consentsPerConsumerMicroservice.ToArray();
                for (var i = 0; i < consentsPerConsumerMicroserviceArray.Length; i++)
                {
                    var consent = consentsPerConsumerMicroserviceArray[i];
                    var consentPrefix = ConfigurationPath.Combine(consentSectionPrefix, $"{i}");
                    Data[ConfigurationPath.Combine(consentPrefix, "consumerTenant")] = consent["tenant"];
                    Data[ConfigurationPath.Combine(consentPrefix, "stream")] = consent["stream"];
                    Data[ConfigurationPath.Combine(consentPrefix, "partition")] = consent["partition"];
                    Data[ConfigurationPath.Combine(consentPrefix, "consent")] = consent["consent"];
                }
            }
        }
    }

    static IEnumerable<KeyValuePair<string, string>> GetData(string rootPath, IConfiguration config)
    {
        var data = new List<KeyValuePair<string, string>>();
        foreach (var section in config.GetChildren())
        {
            data.Add(new KeyValuePair<string, string>(ConfigurationPath.Combine(rootPath, section.Key), section.Value));
            foreach (var subSection in section.GetChildren())
            {
                var subRoot = ConfigurationPath.Combine(rootPath, section.Key, subSection.Key);
                data.Add(new KeyValuePair<string, string>(subRoot, subSection.Value));
                data.AddRange(GetData(subRoot, subSection));
            }
        }
        return data;
    }
}
