// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Contracts;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc.for_ProjectionsGrpcService.given;

public class the_service
{
    protected static Mock<IProjectionsService> projections_service;
    protected static ProjectionsGrpcService grpc_service;

    private Establish context = () =>
    {
        projections_service = new Mock<IProjectionsService>();
        
        grpc_service = new ProjectionsGrpcService(
            projections_service.Object,
            NullLogger.Instance);
    };
}