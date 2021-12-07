// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using ReverseCallClient = Dolittle.Runtime.Services.Clients.ReverseCallClient<
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyClient,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyClientMessage,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyServerMessage,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyConnectArguments,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyConnectResponse,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyRequest,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyResponse>;
using Status = Grpc.Core.Status;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting;

public class and_the_ping_times_out : given.a_reverse_call_client
{
    static Execution.ExecutionContext execution_context;
    static Exception exception;

    Establish context = () =>
    {
        execution_context = given.execution_contexts.create();
        execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
        server_to_client_stream
            .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
            // wait for the keepalive to timeout, then throw the exception mimicking a cancelled connection
            .ThrowsAsync(new RpcException(new Status(StatusCode.Cancelled, "")), ping_interval.Multiply(ReverseCallClient.PingThreshold + 1));
    };

    Because of = () => exception = Catch.Exception(() => reverse_call_client.Connect(new MyConnectArguments(), CancellationToken.None).GetAwaiter().GetResult());

    It should_throw_an_exception = () =>
        exception.ShouldBeOfExactType<PingTimedOut>();
}