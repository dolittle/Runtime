// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_handling;

public class and_connection_was_not_established : given.a_reverse_call_client
{
    static Execution.ExecutionContext execution_context;
    static Exception exception;

    Establish context = () =>
    {
        execution_context = given.execution_contexts.create();
        server_to_client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
        reverse_call_client.Connect(new MyConnectArguments(), execution_context, CancellationToken.None).GetAwaiter().GetResult();
    };

    Because of = () => exception = Catch.Exception(() => reverse_call_client.Handle((request, _, token) => Task.FromResult(new MyResponse()), CancellationToken.None).GetAwaiter().GetResult());

    It should_fail_because_no_connection_was_established = () => exception.ShouldBeOfExactType<ReverseCallClientNotConnected>();
}