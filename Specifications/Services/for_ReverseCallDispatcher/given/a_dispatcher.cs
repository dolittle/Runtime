// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Grpc.Core;
using Moq;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Dolittle.Runtime.Services.ReverseCalls;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given
{
    public class a_dispatcher
    {
        protected static IReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse> dispatcher;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IPingedConnection<MyClientMessage, MyServerMessage>> pinged_connection;

        protected static Mock<IAsyncStreamReader<MyClientMessage>> client_stream;
        protected static Mock<IServerStreamWriter<MyServerMessage>> server_stream;

        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
            execution_context_manager = new();
            pinged_connection = new();
            client_stream = new();
            server_stream = new();
            cancellation_token = new();

            pinged_connection.SetupGet(_ => _.RuntimeStream).Returns(client_stream.Object);
            pinged_connection.SetupGet(_ => _.ClientStream).Returns(server_stream.Object);
            pinged_connection.SetupGet(_ => _.CancellationToken).Returns(cancellation_token);
            dispatcher = new ReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>(
                pinged_connection.Object,
                new protocol(),
                execution_context_manager.Object,
                Mock.Of<ILogger>());
        };
    }
    public class protocol : IConvertReverseCallMessages<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>
    {
        public MyConnectResponse CreateFailedConnectResponse(FailureReason failureMessage)
            => new();

        public ReverseCallArgumentsContext GetArgumentsContext(MyConnectArguments message)
            => message.Context;

        public MyConnectArguments GetConnectArguments(MyClientMessage message)
            => message.Arguments;

        public Pong GetPong(MyClientMessage message)
            => message.Pong;

        public MyResponse GetResponse(MyClientMessage message)
            => message.Response;

        public ReverseCallResponseContext GetResponseContext(MyResponse message)
            => message.Context;

        public void SetConnectResponse(MyConnectResponse arguments, MyServerMessage message)
            => message.ConnectResponse = arguments;

        public void SetPing(MyServerMessage message, Ping ping)
            => message.Ping = ping;

        public void SetRequest(MyRequest request, MyServerMessage message)
            => message.Request = request;

        public void SetRequestContext(ReverseCallRequestContext context, MyRequest request)
            => request.Context = context;
    }
}