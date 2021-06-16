// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting
{
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
                .ThrowsAsync(new RpcException(new(StatusCode.Cancelled, "")), ping_interval.Multiply(3));
        };

        Because of = () => exception = Catch.Exception(() => reverse_call_client.Connect(new(), CancellationToken.None).GetAwaiter().GetResult());

        It should_throw_an_exception = () =>
            exception.ShouldBeOfExactType<PingTimedOut>();
    }
}
