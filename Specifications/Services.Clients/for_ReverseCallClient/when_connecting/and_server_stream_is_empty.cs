// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting;

public class and_server_stream_is_empty : given.a_reverse_call_client
{
    static bool result;
    static Execution.ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = given.execution_contexts.create();
        server_to_client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
    };

    Because of = () => result = reverse_call_client.Connect(new MyConnectArguments(), execution_context, CancellationToken.None).GetAwaiter().GetResult();

    It should_return_false = () => result.Should().BeFalse();
    It should_write_to_server_once = () => client_to_server_stream.Verify(_ => _.WriteAsync(Moq.It.IsAny<MyClientMessage>()), Moq.Times.Once);
    It should_read_from_server_once = () => server_to_client_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_not_set_connect_response = () => reverse_call_client.ConnectResponse.Should().BeNull();
}