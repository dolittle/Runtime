// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Configuration.Runtime;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services that provide configuration for a Runtime.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddRuntimeConfiguration(this ServiceCollection services)
    {
        services.AddTransient<IRuntimeConfiguration, RuntimeConfiguration>();
        services.AddTransient(provider => provider.GetRequiredService<IRuntimeConfiguration>().Resources);
    }
}