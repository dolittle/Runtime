// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    public static IHostBuilder AddGrpcHost(this IHostBuilder builder, EndpointVisibility visibility)
        => builder.AddScopedHost(_ => _.ConfigureWebHost(grpcHost =>
        {
            grpcHost.UseKestrel();

            grpcHost.ConfigureServices(services =>
            {
                services.AddKestrelConfigurationFor(visibility);
                services.AddGrpc();
                services.AddGrpcReflection();
            });

            grpcHost.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDiscoveredGrpcServicesOf(visibility);
                    endpoints.MapGrpcReflectionService();
                });
            });
        }));

}
