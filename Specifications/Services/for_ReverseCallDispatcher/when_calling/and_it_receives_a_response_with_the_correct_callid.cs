// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling;

public class and_it_receives_a_response_with_the_correct_callid : given.a_dispatcher
{
    static MyRequest request;
    static ExecutionContext execution_context_in_request;
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

                execution_context_in_request = server_message.Request.Context.ExecutionContext.ToExecutionContext();
                response_from_client.Context = new ReverseCallResponseContext
                    { CallId = server_message.Request.Context.CallId };

                stream_reader.ReceiveMessage(new MyClientMessage() { Response = response_from_client });
            })
            .Returns(Task.FromResult(true));

        Task.Run(() => dispatcher.Accept(new MyConnectResponse(), CancellationToken.None));
    };

    static MyResponse response;
    Because of = () => response = dispatcher.Call(request, CancellationToken.None).GetAwaiter().GetResult();

    It should_write_a_message_with_the_request = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.Request == request)), Moq.Times.Once);
    It should_set_the_current_execution_context_in_the_request = () => execution_context_in_request.ShouldEqual(execution_context);
    It should_return_the_received_response = () => response.ShouldEqual(response_from_client);
}