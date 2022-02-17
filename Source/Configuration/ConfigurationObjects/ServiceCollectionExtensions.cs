// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDolittleConfigurations(this IServiceCollection services, IConfiguration dolittleConfiguration)
    {
        var json = new ConvertDolittleConfigurationToJson().Convert(dolittleConfiguration);
        var x2 = JsonSerializer.Deserialize<EndpointsConfiguration>(json["endpoints"], new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        var x3 = JsonSerializer.Deserialize<TenantsConfiguration>(json["tenants"], new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        return services;
    }
}
