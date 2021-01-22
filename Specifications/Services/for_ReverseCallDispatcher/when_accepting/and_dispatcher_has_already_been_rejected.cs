// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_accepting
{
    public class and_dispatcher_has_already_been_rejected : given.a_dispatcher
    {
        static MyConnectResponse connect_response;
        static Exception exception;

        Establish context = () =>
        {
            connect_response = new MyConnectResponse();
            client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
            dispatcher.Reject(connect_response, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => dispatcher.Accept(connect_response, CancellationToken.None).GetAwaiter().GetResult());

        It should_fail_because_dispatcher_has_already_been_rejected = () => exception.ShouldBeOfExactType<ReverseCallDispatcherAlreadyRejected>();
    }
}