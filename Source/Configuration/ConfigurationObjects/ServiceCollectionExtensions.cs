// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDolittleConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => new DolittleConfigurations(_.GetRequiredService<IConvertDolittleConfigurationToJson>().Convert(configuration.GetSection("dolittle:runtime"))));
        return services;
    }
}
