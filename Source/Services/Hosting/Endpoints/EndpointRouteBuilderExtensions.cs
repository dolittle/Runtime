// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Services.Hosting.Endpoints;

/// <summary>
/// Extension methods for <see cref="IEndpointRouteBuilder"/> related to gRPC endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps discovered gRPC service implementations for the specified <see cref="EndpointVisibility"/>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map services to.</param>
    /// <param name="visibility">The endpoint visibility to map services for.</param>
    public static void MapDiscoveredGrpcServicesOf(this IEndpointRouteBuilder endpoints, EndpointVisibility visibility)
       => endpoints
           .ServiceProvider.GetRequiredService<ICanConfigureGrpcServicesForEndpoint>()
           .MapServicesForVisibility(endpoints, visibility);
}
