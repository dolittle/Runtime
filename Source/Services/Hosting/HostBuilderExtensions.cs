// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Hosting;
using Dolittle.Runtime.Services.HealthChecks;
using Dolittle.Runtime.Services.Hosting.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/> related to gRPC endpoints.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Configures a scoped host that serves gRPC endpoints of the specified <see cref="EndpointVisibility"/> using Kestrel.
    /// </summary>
    /// <param name="builder">The host builder to add a scoped host to.</param>
    /// <param name="visibility">The endpoint visibility to serve endpoints for.</param>
    /// <returns>The builder for continuation.</returns>
    public static IHostBuilder AddGrpcHost(this IHostBuilder builder, EndpointVisibility visibility)
        => builder
            .AddGrpcEndpointHealthCheck(visibility)
            .AddScopedHost(_ => _.ConfigureWebHost(grpcHost =>
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
                        endpoints.MapGrpcService<HealthService>();
                    });
                });
            }));
}
