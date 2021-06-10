// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_accepting
{
    public class and_the_token_is_cancelled : given.a_dispatcher
    {
        static MyConnectResponse connect_response;
        static CancellationTokenSource cst;
        Establish context = () =>
        {
            cst = new();
            connect_response = new MyConnectResponse();
            client_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                // cancel the token after returning the value
                .Callback(() =>
                {
                    cst.Cancel();
                    cst.Dispose();
                });
        };

        Because of = () =>
        {
            dispatcher.Accept(connect_response, cst.Token).GetAwaiter().GetResult();
        };

        It should_write_one_message = () => server_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyServerMessage>(_ => _.ConnectResponse.Equals(connect_response))), Moq.Times.Once);
        It should_move_client_stream_once = () => client_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}
