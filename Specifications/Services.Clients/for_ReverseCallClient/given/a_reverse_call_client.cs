// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extension.Logging;
using Grpc.Core;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given
{
    public class a_reverse_call_client
    {
        protected static IReverseCallClient<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse> reverse_call_client;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IAsyncStreamReader<MyServerMessage>> server_stream;
        protected static Mock<IClientStreamWriter<MyClientMessage>> client_stream;

        Establish context = () =>
        {
            execution_context_manager = new Mock<IExecutionContextManager>();
            server_stream = new Mock<IAsyncStreamReader<MyServerMessage>>();
            client_stream = new Mock<IClientStreamWriter<MyClientMessage>>();
            reverse_call_client = new ReverseCallClient<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>(
                () => new AsyncDuplexStreamingCall<MyClientMessage, MyServerMessage>(client_stream.Object, server_stream.Object, Task.FromResult(new Metadata()), () => Grpc.Core.Status.DefaultSuccess, () => new Metadata(), () => { }),
                (message, argument) => message.Arguments = argument,
                message => message.ConnectResponse,
                message => message.Request,
                (message, response) => message.Response = response,
                (arguments, context) => arguments.Context = context,
                request => request.Context,
                (response, context) => response.Context = context,
                message => message.Ping,
                (message, pong) => message.Pong = pong,
                new TimeSpan(0, 0, 0, 0, 500),
                execution_context_manager.Object,
                Mock.Of<ILogger>());
        };
    }
}
