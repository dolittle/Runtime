// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_rejecting
{
    public class and_it_has_already_been_rejected : given.a_dispatcher
    {
        static MyConnectResponse connect_response;
        static Exception exception;

        Establish context = () =>
        {
            connect_response = new MyConnectResponse();
            dispatcher.Reject(connect_response, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => dispatcher.Reject(new MyConnectResponse(), CancellationToken.None).GetAwaiter().GetResult());

        It should_fail_because_dispatcher_has_already_been_rejected = () => exception.ShouldBeOfExactType<ReverseCallDispatcherAlreadyRejected>();
        It should_write_the_first_response = () => runtime_to_client_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.ConnectResponse == connect_response)), Moq.Times.Once);
        It should_not_write_anything_else = () => runtime_to_client_stream.VerifyNoOtherCalls();
    }
}
