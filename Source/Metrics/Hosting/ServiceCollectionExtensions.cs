// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Metrics.Hosting;

public static class ServiceCollectionExtensions
{
    public static void AddKestrelConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<KestrelServerOptions>, KestrelConfiguration>();
    }
}
