// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Services.Hosting;

public static class EndpointRouteBuilderExtensions
{
    public static void MapDiscoveredGrpcServicesOf(this IEndpointRouteBuilder endpoints, EndpointVisibility visibility, Action<GrpcServiceEndpointConventionBuilder> configure = null)
    {
        var definitions = endpoints.ServiceProvider
            .GetRequiredService<IEnumerable<ServiceDefinition>>()
            .Where(_ => _.Visibility == visibility);

        foreach (var definition in definitions)
        {
            var builder = endpoints.MapDynamicGrpcService(definition.ImplementationType);
            configure?.Invoke(builder);
        }
    }
    
    static GrpcServiceEndpointConventionBuilder MapDynamicGrpcService(this IEndpointRouteBuilder builder, Type service)
    {
        // TODO: Cleanup with some null checking
        var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService));
        var methodForServiceType = method.MakeGenericMethod(service);
        return methodForServiceType.Invoke(null, new object[] { builder }) as GrpcServiceEndpointConventionBuilder;
    }
}
