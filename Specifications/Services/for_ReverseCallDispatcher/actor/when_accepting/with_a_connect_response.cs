// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.actor.when_accepting;

public class with_a_connect_response : given.a_dispatcher
{
    static MyConnectResponse connect_response;
    static MyConnectResponse received_connect_response;

    Establish context = () => connect_response = new MyConnectResponse();

    Because of = () => dispatcher.Accept(connect_response, new CancellationTokenSource(200).Token).GetAwaiter().GetResult();

    It should_write_the_connection_response = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.ConnectResponse == connect_response)), Moq.Times.Once);
    It should_write_a_connection_response = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()), Moq.Times.Once);
}