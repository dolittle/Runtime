// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.actor.when_calling;

public class and_it_receives_a_response_with_another_callid : given.a_dispatcher
{
    static MyRequest request;
    static MyResponse response_from_client;

    Establish context = () =>
    {
        request = new MyRequest();
        response_from_client = new MyResponse();

        var stream_reader = new MockStreamReader();

        pinged_connection
            .SetupGet(_ => _.RuntimeStream)
            .Returns(stream_reader);

        runtime_to_client_stream
            .Setup(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()))
            .Callback<MyServerMessage>(server_message =>
            {
                if (server_message.Request != request)
                {
                    return;
                }

                response_from_client.Context = new ReverseCallResponseContext
                    { CallId = ReverseCallId.New().ToProtobuf() };

                stream_reader.ReceiveMessage(new MyClientMessage() { Response = response_from_client });
            })
            .Returns(Task.FromResult(true));

        _ = dispatcher.Accept(new MyConnectResponse(), CancellationToken.None);
    };

    static Task<MyResponse> response;
    Because of = () =>
    {
        response = dispatcher.Call(request, execution_context, CancellationToken.None);
        Thread.Sleep(50);
    };

    It should_write_a_message_with_the_request = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.Request == request)), Moq.Times.Once);
    It should_not_have_received_a_response = () => response.IsCompleted.ShouldBeFalse();
}