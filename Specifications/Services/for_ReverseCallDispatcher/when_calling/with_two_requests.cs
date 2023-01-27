// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Dolittle.Services.Contracts;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling;

[Ignore("This hangs most of the times")]
public class with_two_requests : given.a_dispatcher
{
    static MyRequest first_request;
    static MyRequest second_request;
    static MyResponse response_to_first_from_client;
    static MyResponse response_to_second_from_client;

    Establish context = () =>
    {
        first_request = new MyRequest();
        second_request = new MyRequest();
        response_to_first_from_client = new MyResponse();
        response_to_second_from_client = new MyResponse();

        var stream_reader = new MockStreamReader();

        pinged_connection
            .SetupGet(_ => _.RuntimeStream)
            .Returns(stream_reader);

        runtime_to_client_stream
            .Setup(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()))
            .Callback<MyServerMessage>(server_message =>
            {
                if (server_message.Request == first_request)
                {
                    response_to_first_from_client.Context = new ReverseCallResponseContext
                        { CallId = server_message.Request.Context.CallId };
                    return;
                }

                if (server_message.Request == second_request)
                {
                    response_to_second_from_client.Context = new ReverseCallResponseContext
                        { CallId = server_message.Request.Context.CallId };

                    stream_reader.ReceiveMessage(new MyClientMessage() { Response = response_to_second_from_client });
                    stream_reader.ReceiveMessage(new MyClientMessage() { Response = response_to_first_from_client });
                }

            })
            .Returns(Task.FromResult(true));

        Task.Run(() => dispatcher.Accept(new MyConnectResponse(), CancellationToken.None));
    };

    static MyResponse first_response;
    static MyResponse second_response;
    Because of = () =>
    {
        var first_call = dispatcher.Call(first_request, execution_context, CancellationToken.None);
        var second_call = dispatcher.Call(second_request, execution_context, CancellationToken.None);

        first_response = first_call.GetAwaiter().GetResult();
        second_response = second_call.GetAwaiter().GetResult();
    };

    It should_write_a_message_with_the_first_request = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.Request == first_request)), Moq.Times.Once);
    It should_write_a_message_with_the_second_request = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.Request == second_request)), Moq.Times.Once);
    It should_return_the_first_response_to_the_first_request = () => first_response.Should().Be(response_to_first_from_client);
    It should_return_the_second_response_to_the_second_request = () => second_response.Should().Be(response_to_second_from_client);
}