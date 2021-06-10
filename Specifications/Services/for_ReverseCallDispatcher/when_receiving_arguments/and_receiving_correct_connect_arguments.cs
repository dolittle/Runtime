// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using System;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_receiving_arguments
{
    public class and_receiving_correct_connect_arguments : given.a_dispatcher
    {
        static bool result;
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

        Because of = () => result = dispatcher.ReceiveArguments(CancellationToken.None).GetAwaiter().GetResult();

        It should_return_true = () => result.ShouldBeTrue();
        It should_have_the_correct_arguments = () => dispatcher.Arguments.ShouldEqual(arguments);
        It should_change_execution_context = () => execution_context_manager
            .Verify(
                _ => _.CurrentFor(
                    execution_context,
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<string>()), Moq.Times.Once);
    }
}
