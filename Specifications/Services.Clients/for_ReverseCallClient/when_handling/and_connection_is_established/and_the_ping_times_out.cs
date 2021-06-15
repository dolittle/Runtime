// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using Contracts = Dolittle.Services.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_accepting.and_connection_is_established
{
    public class and_the_ping_times_out : given.a_reverse_call_client
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
                Context = new Contracts.ReverseCallArgumentsContext
                {
                    ExecutionContext = execution_context.ToProtobuf()
                }
            }, CancellationToken.None).GetAwaiter().GetResult();

            server_to_client_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                // wait for the keepalive to timeout, then throw the exception mimicking a cancelled connection
                .Callback(() => Thread.Sleep(ping_interval.Multiply(3)))
                .Throws(new RpcException(new(StatusCode.Cancelled, "")));
        };

        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
            reverse_call_client.Handle(callback, CancellationToken.None).GetAwaiter().GetResult());

        It should_not_call_the_callback = () =>
            callback_was_called.ShouldBeFalse();
        It should_throw_an_exception = () =>
            exception.ShouldBeOfExactType<PingTimedOut>();
    }
}
