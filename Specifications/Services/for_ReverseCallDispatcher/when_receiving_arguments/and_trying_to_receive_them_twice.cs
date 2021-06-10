// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Dolittle.Services.Contracts;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_receiving_arguments
{
    public class and_trying_to_receive_them_twice : given.a_dispatcher
    {
        static MyConnectArguments arguments;

        Establish context = () =>
        {
            arguments = new MyConnectArguments
            {
                Context = new ReverseCallArgumentsContext
                {
                    ExecutionContext = execution_context.ToProtobuf(),
                    PingInterval = Duration.FromTimeSpan(new TimeSpan(0, 0, 1))
                }
            };

            client_to_runtime_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            client_to_runtime_stream
                .SetupGet(_ => _.Current)
                .Returns(new MyClientMessage { Arguments = arguments });
        };
        static Exception exception;
        Because of = () =>
        {
            dispatcher.ReceiveArguments(CancellationToken.None).GetAwaiter().GetResult();
            exception = Catch.Exception(() => dispatcher.ReceiveArguments(CancellationToken.None).GetAwaiter().GetResult());
        };

        It should_throw_exception_on_second_try = () => exception.ShouldBeOfExactType<ReverseCallDispatcherAlreadyTriedToReceiveArguments>();
        It shouldnt_have_tried_to_read_the_stream_again = () => client_to_runtime_stream.Verify(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()), Times.Once);
    }
}
