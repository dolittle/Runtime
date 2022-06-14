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
                MapIntoRoot(ConfigurationPath.Combine("endpoints", "metrics"), config);
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
        foreach (var kvp in GetData(Constants.CombineWithDolittleConfigRoot(sectionRoot), config))
        {
            Data.Add(kvp);
        }
    }
    
    void MapResources(IConfiguration config)
    {
        foreach (var resourceForTenant in config.GetChildren().Select(_ => config.GetSection(_.Key)))
        {
            foreach (var kvp in GetData(
                         Constants.CombineWithDolittleConfigRoot("tenants", resourceForTenant.Key, "resources"),
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
            var sectionPrefix = Constants.CombineWithDolittleConfigRoot("tenants",microservicesForTenant.Key, "eventHorizons");
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
                    Data.Add(ConfigurationPath.Combine(consentPrefix, "consumerTenant"), consent["tenant"]);
                    Data.Add(ConfigurationPath.Combine(consentPrefix, "stream"), consent["stream"]);
                    Data.Add(ConfigurationPath.Combine(consentPrefix, "partition"), consent["partition"]);
                    Data.Add(ConfigurationPath.Combine(consentPrefix, "consent"), consent["consent"]);
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
