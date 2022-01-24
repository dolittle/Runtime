// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Execution.Contracts.ExecutionContext;
using Version = Dolittle.Versioning.Contracts.Version;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc.for_ProjectionsGrpcService.given;

public class the_service_and_get_all_request : the_service
{
    protected static GetAllRequest request;
    protected static ServerCallContext call_context;

    protected static Mock<IServerStreamWriter<GetAllResponse>> server_stream_writer;

    private Establish context = () =>
    {
        request = new GetAllRequest
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = new ExecutionContext
                {
                    Environment = "Environment",
                    Version = new Version
                    {
                        Major = 1,
                        Minor = 2,
                        Patch = 3,
                        Build = 4,
                        PreReleaseString = "5",
                    },
                    MicroserviceId = Guid.Parse("684ff8ca-45f9-4487-a824-e3c4025ee67a").ToProtobuf(),
                    TenantId = Guid.Parse("4781ad2b-c81e-4648-8cff-313c263b9b0e").ToProtobuf(),
                    CorrelationId = Guid.Parse("8dfc8a6c-9a9b-4dc6-940f-bdf8d593bdd8").ToProtobuf(),
                },
                HeadId = Guid.Parse("ddf68c9b-36d4-41c5-9479-57473030b9ac").ToProtobuf(),
            },
            ProjectionId = Guid.Parse("3150b10b-db09-43a6-bdba-a0c9642ef569").ToProtobuf(),
            ScopeId = Guid.Parse("be1ac604-12de-4a14-bce0-45fdeef30bc8").ToProtobuf(),
        };

        call_context = Mock.Of<ServerCallContext>();

        server_stream_writer = new Mock<IServerStreamWriter<GetAllResponse>>();
    };

    protected static string GenerateRandomString(int length)
        => string.Join("", Enumerable.Repeat(0, length).Select(_ => (char) Random.Shared.Next(127)));
}