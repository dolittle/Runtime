// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;
using Dolittle.Runtime.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    public static IHostBuilder AddGrpcHost(this IHostBuilder builder, EndpointVisibility visibility, int port)
        => builder.AddScopedHost(_ => _.ConfigureWebHost(grpcHost =>
        {
            grpcHost.UseKestrel(_ => _.Listen(IPAddress.Any, port, _ => _.Protocols = HttpProtocols.Http2));
            
            grpcHost.ConfigureServices(services =>
            {
                services.AddGrpc();
                services.AddGrpcReflection();
            });

            grpcHost.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    var implementedServices = app
                        .ApplicationServices
                        .GetRequiredService<IEnumerable<ServiceDefinition>>()
                        .Where(_ => _.Visibility == visibility);

                    foreach (var definition in implementedServices)
                    {
                        endpoints.MapDynamicGrpcService(definition.ImplementationType);
                    }

                    endpoints.MapGrpcReflectionService();
                });
            });
        }));

    static GrpcServiceEndpointConventionBuilder MapDynamicGrpcService(this IEndpointRouteBuilder builder, Type service)
    {
        Console.WriteLine($"Mapping grpc endpoint for {service}");
        var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService));
        var methodForServiceType = method.MakeGenericMethod(service);
        return methodForServiceType.Invoke(null, new object[] { builder }) as GrpcServiceEndpointConventionBuilder;
    }
}
