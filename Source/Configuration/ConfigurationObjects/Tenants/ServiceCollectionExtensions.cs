// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Tenants;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTenantsConfig(this IServiceCollection services)
    {
        services.AddSingleton(sp => sp.GetRequiredService<DolittleConfigurations>().GetFor<TenantsConfiguration>());
        return services;
    }
}
