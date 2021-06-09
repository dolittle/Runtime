// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_rejecting
{
    public class and_its_gets_rejected_again : given.a_dispatcher
    {
        static MyConnectResponse connect_response;
        static Exception exception;

        Establish context = () =>
        {
            connect_response = new MyConnectResponse();
            dispatcher.Reject(connect_response, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => dispatcher.Reject(connect_response, CancellationToken.None).GetAwaiter().GetResult());

        It should_fail_because_dispatcher_has_already_been_rejected = () => exception.ShouldBeOfExactType<ReverseCallDispatcherAlreadyRejected>();
    }
}
