// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Routing;

namespace Dolittle.Runtime.Services.Hosting.Endpoints;

/// <summary>
/// Defines a system that can map implemented gRPC services to a <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public interface ICanConfigureGrpcServicesForEndpoint
{
    /// <summary>
    /// Maps the services for the specified <see cref="EndpointVisibility"/> to the provided <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to use to map the services.</param>
    /// <param name="visibility">The endpoint visibility to map services for.</param>
    void MapServicesForVisibility(IEndpointRouteBuilder endpoints, EndpointVisibility visibility);
}
