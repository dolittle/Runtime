// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Services.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_accepting.and_connection_is_established
{
    public class and_we_get_a_request : given.a_reverse_call_client
    {
        static Execution.ExecutionContext execution_context;
        // static Mock<ReverseCallHandler<MyRequest, MyResponse>> callback;
        static ReverseCallHandler<MyRequest, MyResponse> callback;
        static bool callback_was_called;
        static MyRequest callback_argument;
        static MyServerMessage server_request;
        static MyRequest request;
        static MyResponse response;
        static Guid call_id;
        static CancellationTokenSource cts;

        Establish context = () =>
        {
            cts = new();
            execution_context = given.execution_contexts.create();
            execution_context_manager
                .SetupGet(_ => _.Current)
                .Returns(execution_context);

            var move_counter = 0;
            server_to_client_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                // stop the reading after 2 reads, one for the Connect and one for the Handle
                .Returns(() =>
                {
                    move_counter++;
                    if (move_counter < 3)
                    {
                        return Task.FromResult(true);
                    }
                    return Task.FromResult(false);
                });
            server_to_client_stream
                .SetupGet(_ => _.Current)
                .Returns(new MyServerMessage { ConnectResponse = new MyConnectResponse() });

            call_id = Guid.Parse("6e53a922-7207-49f7-b5fd-e8ec159fa4db");
            request = new()
            {
                Context = new()
                {
                    ExecutionContext = execution_context.ToProtobuf(),
                    CallId = call_id.ToProtobuf()
                }
            };
            server_request = new MyServerMessage
            {
                Request = request
            };

            response = new();

            callback = (request, token) =>
            {
                callback_was_called = true;
                callback_argument = request;
                return Task.FromResult(response);
            };

            client_to_server_stream
                .Setup(_ => _.WriteAsync(Moq.It.IsAny<MyClientMessage>()))
                .Returns(Task.FromResult(true))
                .Callback<MyClientMessage>(message =>
                {
                    if (message.Response != default)
                    {
                        cts.Cancel();
                    }
                });

            var connect_response = reverse_call_client.Connect(new MyConnectArguments
            {
                Context = new Contracts.ReverseCallArgumentsContext
                {
                    ExecutionContext = execution_context.ToProtobuf()
                }
            }, CancellationToken.None).GetAwaiter().GetResult();
            server_to_client_stream
                .SetupGet(_ => _.Current)
                .Returns(server_request);
        };

        Because of = () =>
        {
            reverse_call_client.Handle(callback, CancellationToken.None).GetAwaiter().GetResult();
            Task.Run(() =>
            {
                try
                {
                    Task.Delay(2000, cts.Token).GetAwaiter().GetResult();
                }
                catch
                {
                }
            }).GetAwaiter().GetResult();
        };

        It should_call_the_callback = () =>
            callback_was_called.ShouldBeTrue();
        It should_call_the_callback_with_the_request = () =>
            callback_argument.ShouldEqual(request);
        It should_write_a_response = () =>
            client_to_server_stream
                .Verify(_ => _.WriteAsync(Moq.It.Is<MyClientMessage>(_ => _.Response != default)),
                    Times.Once());
        It should_write_the_same_call_id_to_the_response = () =>
            client_to_server_stream
                .Verify(_ => _.WriteAsync(
                    Moq.It.Is<MyClientMessage>(_ =>
                        _.Response != default
                        && _.Response.Context.CallId.Equals(call_id.ToProtobuf()))),
                    Times.Once());
    }
}
