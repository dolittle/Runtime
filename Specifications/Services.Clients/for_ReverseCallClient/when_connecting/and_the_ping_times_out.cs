// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Services.Contracts;
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
                // .Callback(() =>
                // {
                //     Console.WriteLine("STARTING CALLBSCK");
                //     Thread.Sleep(ping_interval.Multiply(3));
                //     Console.WriteLine("DONE CALLBSCK");
                // })
                .ThrowsAsync(new RpcException(new(StatusCode.Cancelled, "")), ping_interval.Multiply(3));
            // .Returns(Task.FromException(new RpcException(new(StatusCode.Cancelled, ""))));
        };

        Because of = () =>
        {
            Console.WriteLine("Starting Ping Timeout Because");
            // exception = Catch.Exception(() => reverse_call_client.Connect(new(), CancellationToken.None).GetAwaiter().GetResult());
            try
            {
                reverse_call_client.Connect(new(), CancellationToken.None).GetAwaiter().GetResult();
                // Console.WriteLine($"Result was {result}");
            }
            catch (Exception ex)
            {
                exception = ex;
                Console.WriteLine(ex);
            }
            Console.WriteLine("Ending Ping Timeout Because");
        };

        It should_throw_an_exception = () =>
            exception.ShouldBeOfExactType<PingTimedOut>();
    }
}
