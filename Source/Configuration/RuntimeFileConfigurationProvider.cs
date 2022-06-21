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
/// from a unified configuration json file.
/// </summary>
public class RuntimeFileConfigurationProvider : ConfigurationProvider
{
    readonly IFileInfo _runtimeConfigFile;
    readonly IFileProvider? _legacyFilesProvider;
    readonly Func<IConfigurationBuilder, IFileInfo, IConfigurationRoot> _buildConfigurationFromFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeFileConfigurationProvider"/> class.
    /// </summary>
    /// <param name="runtimeConfigFile"></param>
    /// <param name="legacyFilesProvider"></param>
    /// <param name="buildConfigurationFromFile"></param>
    public RuntimeFileConfigurationProvider(IFileInfo runtimeConfigFile, IFileProvider? legacyFilesProvider, Func<IConfigurationBuilder, IFileInfo, IConfigurationRoot> buildConfigurationFromFile)
    {
        _runtimeConfigFile = runtimeConfigFile;
        _legacyFilesProvider = legacyFilesProvider;
        _buildConfigurationFromFile = buildConfigurationFromFile;
    }

    /// <inheritdoc />
    public override void Load()
    {
        MapRuntimeConfigFile();
        MapLegacyDolittleFiles();
    }

    void MapRuntimeConfigFile()
    {
        AddAllFromMap(GetData(Constants.DolittleConfigSectionRoot, _buildConfigurationFromFile(new ConfigurationBuilder(), _runtimeConfigFile)));
    }
    
    void MapLegacyDolittleFiles()
    {
        foreach (var file in _legacyFilesProvider?.GetDirectoryContents("/") ?? Enumerable.Empty<IFileInfo>())
        {
            MapLegacyDolittleFile(Path.GetFileNameWithoutExtension(file.Name), _buildConfigurationFromFile(new ConfigurationBuilder(), file));
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
                foreach (var key in Data.Keys.Where(_ => _.StartsWith(Constants.CombineWithDolittleConfigRoot("tenants"), StringComparison.InvariantCulture)))
                {
                    Data.Remove(key);
                }
                MapResources(config);
                break;
            case "event-horizon-consents":
                MapEventHorizonConsents(config);
                break;
        }
    }

    void AddAllFromMap(IEnumerable<KeyValuePair<string, string>> map)
    {
        foreach (var (key, value) in map)
        {
            AddOrReplace(key, value);
        }
    }

    void AddOrReplace(string key, string value)
    {
        if (Data.TryAdd(key, value))
        {
            return;
        }
        Data.Remove(key);
        Data.Add(key, value);
    }
    
    void MapIntoRoot(string sectionRoot, IConfiguration config)
    {
        AddAllFromMap(GetData(Constants.CombineWithDolittleConfigRoot(sectionRoot), config));
    }
    
    void MapResources(IConfiguration config)
    {
        foreach (var resourceForTenant in config.GetChildren().Select(_ => config.GetSection(_.Key)))
        {
            AddAllFromMap(GetData(
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
                    AddOrReplace(ConfigurationPath.Combine(consentPrefix, "consumerTenant"), consent["tenant"]);
                    AddOrReplace(ConfigurationPath.Combine(consentPrefix, "stream"), consent["stream"]);
                    AddOrReplace(ConfigurationPath.Combine(consentPrefix, "partition"), consent["partition"]);
                    AddOrReplace(ConfigurationPath.Combine(consentPrefix, "consent"), consent["consent"]);
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
