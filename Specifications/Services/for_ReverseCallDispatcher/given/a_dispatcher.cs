// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.ReverseCalls;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;

public class a_dispatcher
{
    protected static IReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse> dispatcher;
    protected static Mock<ICreateExecutionContexts> execution_context_creator;
    protected static ExecutionContext execution_context;
    protected static Mock<IPingedConnection<MyClientMessage, MyServerMessage>> pinged_connection;
    protected static Mock<IAsyncStreamReader<MyClientMessage>> client_to_runtime_stream;
    protected static Mock<IServerStreamWriter<MyServerMessage>> runtime_to_client_stream;

    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        execution_context_creator = new Mock<ICreateExecutionContexts>();
        pinged_connection = new Mock<IPingedConnection<MyClientMessage, MyServerMessage>>();
        client_to_runtime_stream = new Mock<IAsyncStreamReader<MyClientMessage>>();
        runtime_to_client_stream = new Mock<IServerStreamWriter<MyServerMessage>>();
        cancellation_token = new CancellationToken();

        pinged_connection.SetupGet(_ => _.RuntimeStream).Returns(client_to_runtime_stream.Object);
        pinged_connection.SetupGet(_ => _.ClientStream).Returns(runtime_to_client_stream.Object);
        pinged_connection.SetupGet(_ => _.CancellationToken).Returns(cancellation_token);
        dispatcher = new ReverseCallDispatcher<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>(
            pinged_connection.Object,
            new MyProtocol(),
            execution_context_creator.Object,
            Mock.Of<ILogger>());

        execution_context = execution_contexts.create();
        execution_context_creator
            .Setup(_ => _.TryCreateUsing(Moq.It.IsAny<ExecutionContext>()))
            .Returns(execution_context);
    };
}