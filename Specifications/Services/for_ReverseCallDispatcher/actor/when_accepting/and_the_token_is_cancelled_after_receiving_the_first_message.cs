// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.actor.when_accepting;

public class and_the_token_is_cancelled_after_receiving_the_first_message : given.a_dispatcher
{
    static CancellationTokenSource cts;
    Establish context = () =>
    {
        cts = new CancellationTokenSource();
        client_to_runtime_stream
            .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Callback(() => cts.Cancel());
    };

    static Task task;
    Because of = () =>
    {
        task = dispatcher.Accept(new MyConnectResponse(), cts.Token);
        task.GetAwaiter().GetResult();
    };

    It should_move_client_stream_once = () => client_to_runtime_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_return_from_the_accept_call = () => task.IsCompleted.ShouldBeTrue();
}