// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Builder;

namespace Dolittle.Runtime.Services.Hosting.Endpoints;

/// <summary>
/// The exception that gets throw when the <see cref="GrpcEndpointRouteBuilderExtensions.MapGrpcService{TService}"/> method could not be found by reflection.
/// </summary>
public class CouldNotFindEndpointRouteBuilderMapGrpcServiceMethod : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotFindEndpointRouteBuilderMapGrpcServiceMethod"/> class.
    /// </summary>
    public CouldNotFindEndpointRouteBuilderMapGrpcServiceMethod()
        : base($"Could not find the {nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService)} method using reflection. This method is required to map gRPC services by discovery")
    {
    }
}
