// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_rejecting
{
    public class and_its_gets_rejected : given.a_response
    {
        static MyConnectResponse connect_response;

        Establish context = () =>
        {
            connect_response = new MyConnectResponse();
        };

        Because of = () => dispatcher.Reject(connect_response, CancellationToken.None).GetAwaiter().GetResult();

        It should_write_the_response = () => connect_response_message.ConnectResponse.ShouldEqual(connect_response);
    }
}
