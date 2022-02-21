// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.Hosting;

public static class ServiceCollectionExtensions
{

    public static void AddKestrelConfigurationFor(this IServiceCollection services, EndpointVisibility visibility)
    {
        services.AddSingleton<IConfigureOptions<KestrelServerOptions>>(_ => 
            new KestrelConfiguration(
                _.GetRequiredService<IOptions<EndpointsConfiguration>>(),
                visibility));
    }
}
