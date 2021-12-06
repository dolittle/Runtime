// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_handling.and_connection_is_established
{
    public class and_we_get_2_pings_and_a_request : given.a_reverse_call_client
    {
        static Execution.ExecutionContext execution_context;
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

            server_to_client_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(true));

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
                Context = new Dolittle.Services.Contracts.ReverseCallArgumentsContext
                {
                    ExecutionContext = execution_context.ToProtobuf()
                }
            }, CancellationToken.None).GetAwaiter().GetResult();

            var ping_message = new MyServerMessage
            {
                Ping = new()
            };
            server_to_client_stream
                .SetupSequence(_ => _.Current)
                .Returns(ping_message)
                .Returns(ping_message)
                .Returns(() =>
                {
                    // setup all the subsequent gest, otherwise the next get to Current returns null
                    server_to_client_stream
                        .SetupGet(_ => _.Current)
                        .Returns(server_request);
                    // stop reading more
                    server_to_client_stream
                        .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                        .Returns(() => Task.FromResult(false));
                    return server_request;
                });
        };

        Because of = () =>
        {
            reverse_call_client.Handle(callback, CancellationToken.None).GetAwaiter().GetResult();
            try
            {
                Task.Delay(500, cts.Token).GetAwaiter().GetResult();
            }
            catch
            {
            }
        };

        It should_respond_with_a_pong_twice = () => client_to_server_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyClientMessage>(_ => _.Pong != default)), Moq.Times.Exactly(2));
        It should_call_the_callback = () =>
            callback_was_called.ShouldBeTrue();
        It should_call_the_callback_with_the_request = () =>
            callback_argument.ShouldEqual(request);
    }
}
