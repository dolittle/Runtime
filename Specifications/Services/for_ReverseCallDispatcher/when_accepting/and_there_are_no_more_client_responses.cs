// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_accepting;

public class and_there_are_no_more_client_responses : given.a_dispatcher
{
    static MyConnectResponse connect_response;
    Establish context = () =>
    {
        connect_response = new MyConnectResponse();
        client_to_runtime_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
    };

    Because of = () =>
    {
        dispatcher.Accept(connect_response, CancellationToken.None).GetAwaiter().GetResult();
    };

    It should_write_one_message = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.ConnectResponse.Equals(connect_response))), Moq.Times.Once);
    It should_move_client_stream_once = () => client_to_runtime_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
}