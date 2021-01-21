// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting
{
    public class and_server_stream_responds_with_connect_response : given.a_reverse_call_client
    {
        static bool result;
        static Execution.ExecutionContext execution_context;
        static MyConnectResponse connect_response;
        static MyConnectArguments connect_arguments;

        Establish context = () =>
        {
            connect_arguments = new MyConnectArguments();
            connect_response = new MyConnectResponse();
            execution_context = given.execution_contexts.create();
            execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
            server_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            server_stream.SetupGet(_ => _.Current).Returns(new MyServerMessage { ConnectResponse = connect_response });
        };

        Because of = () => result = reverse_call_client.Connect(connect_arguments, CancellationToken.None).GetAwaiter().GetResult();

        It should_return_true = () => result.ShouldBeTrue();
        It should_write_to_server_once = () => client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyClientMessage>(_ => _.Arguments.Equals(connect_arguments))), Moq.Times.Once);
        It should_read_from_server_once = () => server_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_set_correct_connect_response = () => reverse_call_client.ConnectResponse.ShouldEqual(connect_response);
    }
}