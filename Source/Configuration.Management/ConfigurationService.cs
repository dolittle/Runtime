// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.Management.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

namespace Dolittle.Runtime.Configuration.Management;

/// <summary>
/// Represents an implementation of <see cref="Configuration.ConfigurationBase"/>.
/// </summary>
[ManagementService, ManagementWebService]
public class ConfigurationService : Contracts.Configuration.ConfigurationBase
{
    readonly IDolittleConfiguration _dolittleConfiguration;
    readonly ILogger _logger;

    public ConfigurationService(IDolittleConfiguration dolittleConfiguration, ILogger logger)
    {
        _dolittleConfiguration = dolittleConfiguration;
        _logger = logger;
    }
    /// <inheritdoc />
    public override Task<GetConfigurationYamlResponse> GetConfigurationYaml(GetConfigurationYamlRequest request, ServerCallContext context)
    {
        _logger.GettingConfigurationYaml();
        var yamlString = new Serializer().Serialize(_dolittleConfiguration.Config);
        return Task.FromResult(new GetConfigurationYamlResponse{Value = yamlString});
    }

    
}
