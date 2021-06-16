// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ReverseCallClient = Dolittle.Runtime.Services.Clients.ReverseCallClient<
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyClient,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyClientMessage,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyServerMessage,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyConnectArguments,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyConnectResponse,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyRequest,
                            Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client.MyResponse>;
namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given
{
    public class a_reverse_call_client
    {
        protected static ReverseCallClient reverse_call_client;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IAsyncStreamReader<MyServerMessage>> server_to_client_stream;
        protected static Mock<IClientStreamWriter<MyClientMessage>> client_to_server_stream;
        protected static TimeSpan ping_interval;

        Establish context = () =>
        {
            execution_context_manager = new();
            server_to_client_stream = new();
            client_to_server_stream = new();
            ping_interval = new(0, 0, 0, 0, 500);

            reverse_call_client = new ReverseCallClient(
                new MyProtocol(),
                new MyClient(server_to_client_stream, client_to_server_stream),
                ping_interval,
                execution_context_manager.Object,
                Mock.Of<IMetricsCollector>(),
                Mock.Of<ILogger>());
        };
    }
}
