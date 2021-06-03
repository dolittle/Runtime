// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given
{
    public class a_reverse_call_client
    {
        protected static IReverseCallClient<MyConnectArguments, MyConnectResponse, MyRequest, MyResponse> reverse_call_client;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IAsyncStreamReader<MyServerMessage>> server_stream;
        protected static Mock<IClientStreamWriter<MyClientMessage>> client_stream;

        Establish context = () =>
        {
            execution_context_manager = new Mock<IExecutionContextManager>();
            server_stream = new Mock<IAsyncStreamReader<MyServerMessage>>();
            client_stream = new Mock<IClientStreamWriter<MyClientMessage>>();

            reverse_call_client = new ReverseCallClient<MyClient, MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>(
                new MyProtocol(),
                new MyClient(server_stream, client_stream),
                new TimeSpan(0, 0, 0, 0, 500),
                execution_context_manager.Object,
                Mock.Of<ILogger>());
        };
    }
}
