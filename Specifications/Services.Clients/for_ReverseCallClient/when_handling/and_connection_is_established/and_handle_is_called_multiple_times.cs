// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_handling.and_connection_is_established;

public class and_handle_is_called_multiple_times : given.a_reverse_call_client
{
    static bool connection_established;
    static Execution.ExecutionContext execution_context;

    static Exception exception;

    Establish context = () =>
    {
        execution_context = given.execution_contexts.create();
        server_to_client_stream
            .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                if (!connection_established)
                {
                    connection_established = true;
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            });
        server_to_client_stream.SetupGet(_ => _.Current).Returns(new MyServerMessage { ConnectResponse = new MyConnectResponse() });
        reverse_call_client.Connect(new MyConnectArguments { Context = new Dolittle.Services.Contracts.ReverseCallArgumentsContext { ExecutionContext = execution_context.ToProtobuf() } }, execution_context, CancellationToken.None).GetAwaiter().GetResult();
        reverse_call_client.Handle((request, execution_context, token) => Task.FromResult(new MyResponse()), CancellationToken.None).GetAwaiter().GetResult();
    };

    Because of = () => exception = Catch.Exception(() => reverse_call_client.Handle((request, execution_context, token) => Task.FromResult(new MyResponse()), CancellationToken.None).GetAwaiter().GetResult());

    It should_fail_because_handling_has_already_started = () => exception.ShouldBeOfExactType<ReverseCallClientAlreadyStartedHandling>();
}