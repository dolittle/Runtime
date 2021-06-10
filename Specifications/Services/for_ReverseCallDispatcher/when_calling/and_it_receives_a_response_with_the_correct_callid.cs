// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling
{
    public class and_it_receives_a_response_with_the_correct_callid : a_response
    {
        static MyRequest request;
        static MyConnectResponse connect_response;

        Establish context = () =>
        {
            request = new();
            connect_response = new();

            client_message = new()
            {
                Response = new()
                {
                    Context = new()
                }
            };

            // return a blank response to keep the while loop going, this gets changed later to break the loop
            client_stream
                .SetupGet(_ => _.Current)
                .Returns(client_message);
            server_stream
                .Setup(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()))
                .Callback<MyServerMessage>(server_message =>
                {
                    if (server_message.Request?.Context?.CallId != null)
                    {
                        client_message.Response.Context.CallId = server_message.Request.Context.CallId;
                    }
                })
                .Returns(Task.FromResult(true));

            Task.Run(() => dispatcher.Accept(connect_response, CancellationToken.None));
        };

        static MyResponse response;
        Because of = () => response = dispatcher.Call(request, CancellationToken.None).GetAwaiter().GetResult();

        It should_write_a_message_with_the_request = () => server_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.Request == request)), Moq.Times.Once);

        It should_then_write_one_request = () => server_stream
            .Verify(_ =>
                _.WriteAsync(Moq.It.Is<MyServerMessage>(msg =>
                    connect_request_message.Equals(msg))),
                Moq.Times.Once);
        It should_have_the_correct_callid_in_the_response = () =>
            response.Context.CallId.ShouldEqual(client_message.Response.Context.CallId);
        It should_have_the_execution_context_set = () =>
            connect_request_message.Request.Context.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
    }
}
