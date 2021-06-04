// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting
{
    public class multiple_times : given.a_reverse_call_client
    {
        static Execution.ExecutionContext execution_context;
        static MyConnectResponse connect_response;
        static MyConnectArguments connect_arguments;
        static Exception exception;

        Establish context = () =>
        {
            connect_arguments = new MyConnectArguments();
            connect_response = new MyConnectResponse();
            execution_context = given.execution_contexts.create();
            execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
            server_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            server_stream.SetupGet(_ => _.Current).Returns(new MyServerMessage { ConnectResponse = connect_response });

            reverse_call_client.Connect(connect_arguments, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => reverse_call_client.Connect(connect_arguments, CancellationToken.None).GetAwaiter().GetResult());

        It should_fail_because_you_cannot_connect_twice = () => exception.ShouldBeOfExactType<ReverseCallClientAlreadyCalledConnect>();
    }
}