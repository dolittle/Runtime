// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_accepting
{
    public class with_a_connect_response : given.a_dispatcher
    {
        static MyConnectResponse connect_response;

        Establish context = () => connect_response = new();

        Because of = () => dispatcher.Accept(connect_response, CancellationToken.None);

        It should_write_the_connection_response = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.ConnectResponse == connect_response)), Moq.Times.Once);
    }
}
