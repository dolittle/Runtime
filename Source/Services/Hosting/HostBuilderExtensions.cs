// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
                services.AddGrpcHealthChecks()
                    .AddCheck($"{Enum.GetName(typeof(EndpointVisibility), visibility)}HealthCheck", () => HealthCheckResult.Healthy());
                services.AddGrpcReflection();
            });

            grpcHost.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDiscoveredGrpcServicesOf(visibility); // TODO: Make this a little nicer with some logs to show the endpoints
                    endpoints.MapGrpcReflectionService();
                    endpoints.MapGrpcHealthChecksService();
                });
            });
        }));

}
