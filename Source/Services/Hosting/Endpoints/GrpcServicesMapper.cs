// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Hosting.Endpoints;

/// <summary>
/// Represents an implementation of <see cref="ICanConfigureGrpcServicesForEndpoint"/> that maps discovered gRPC service implementations.
/// </summary>
public class DiscoveredServicesMapper : ICanConfigureGrpcServicesForEndpoint
{
    readonly IEnumerable<ServiceDefinition> _services;
    readonly ILogger _logger;
    readonly MethodInfo _mapGrpcServiceMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveredServicesMapper"/> class.
    /// </summary>
    /// <param name="services">The discovered gRPC service implementation definitions.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public DiscoveredServicesMapper(IEnumerable<ServiceDefinition> services, ILogger logger)
    {
        _services = services;
        _logger = logger;
        _mapGrpcServiceMethod = FindMapGrpcServiceMethod();
    }

    /// <inheritdoc />
    public void MapServicesForVisibility(IEndpointRouteBuilder endpoints, EndpointVisibility visibility)
    {
        foreach (var service in _services.Where(_ => _.Visibility == visibility))
        {
            _logger.MappingDiscoveredGrpcService(service.ImplementationType, visibility);
            MapDynamicGrpcService(endpoints, service.ImplementationType);
        }
    }
    
    void MapDynamicGrpcService(IEndpointRouteBuilder builder, Type service)
    {
        var methodForServiceType = _mapGrpcServiceMethod.MakeGenericMethod(service);
        methodForServiceType.Invoke(null, new object[] { builder });
    }

    static MethodInfo FindMapGrpcServiceMethod()
    {
        var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService), new []{ typeof(IEndpointRouteBuilder) });
        if (method == default)
        {
            throw new CouldNotFindEndpointRouteBuilderMapGrpcServiceMethod();
        }

        return method;
    }
}
