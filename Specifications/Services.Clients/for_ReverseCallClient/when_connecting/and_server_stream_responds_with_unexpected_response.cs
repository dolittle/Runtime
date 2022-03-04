// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting;

public class and_server_stream_responds_with_unexpected_response : given.a_reverse_call_client
{
    static bool result;
    static Execution.ExecutionContext execution_context;
    static MyConnectArguments connect_arguments;

    Establish context = () =>
    {
        connect_arguments = new MyConnectArguments();
        execution_context = given.execution_contexts.create();
        server_to_client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
        server_to_client_stream.SetupGet(_ => _.Current).Returns(new MyServerMessage { Request = new MyRequest() });
    };

    Because of = () => result = reverse_call_client.Connect(connect_arguments, execution_context, CancellationToken.None).GetAwaiter().GetResult();

    It should_return_false = () => result.ShouldBeFalse();
    It should_write_to_server_once = () => client_to_server_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyClientMessage>(_ => _.Arguments.Equals(connect_arguments))), Moq.Times.Once);
    It should_read_from_server_once = () => server_to_client_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_not_set_connect_response = () => reverse_call_client.ConnectResponse.ShouldBeNull();
}