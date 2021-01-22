// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Logging;
using Grpc.Core;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given
{
    public class a_dispatcher
    {
        protected static IReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse> dispatcher;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IAsyncStreamReader<MyClientMessage>> client_stream;
        protected static Mock<IServerStreamWriter<MyServerMessage>> server_stream;

        Establish context = () =>
        {
            execution_context_manager = new Mock<IExecutionContextManager>();
            client_stream = new Mock<IAsyncStreamReader<MyClientMessage>>();
            server_stream = new Mock<IServerStreamWriter<MyServerMessage>>();
            dispatcher = new ReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>(
                client_stream.Object,
                server_stream.Object,
                new CallContext(),
                message => message.Arguments,
                (message, response) => message.ConnectResponse = response,
                (message, request) => message.Request = request,
                message => message.Response,
                arguments => arguments.Context,
                (request, context) => request.Context = context,
                response => response.Context,
                (message, ping) => message.Ping = ping,
                message => message.Pong,
                execution_context_manager.Object,
                Mock.Of<ILogger>());
        };
    }
}