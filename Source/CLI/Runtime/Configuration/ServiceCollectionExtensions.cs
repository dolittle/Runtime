// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Runtime.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to management of Aggregates.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddConfigurationServices(this IServiceCollection services)
    {
        services.AddTransient<IManagementClient, ManagementClient>();
    }
}
