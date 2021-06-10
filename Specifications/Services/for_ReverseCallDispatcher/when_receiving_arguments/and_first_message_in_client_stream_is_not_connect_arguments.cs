// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_receiving_arguments
{
    public class and_first_message_in_client_stream_is_not_connect_arguments : a_dispatcher
    {
        static bool result;

        Establish context = () =>
        {
            client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
            client_stream.SetupGet(_ => _.Current).Returns(new MyClientMessage());
        };

        Because of = () => result = dispatcher.ReceiveArguments(CancellationToken.None).GetAwaiter().GetResult();

        It should_return_false = () => result.ShouldBeFalse();
        It should_not_set_arguments = () => dispatcher.Arguments.ShouldBeNull();

        It should_not_change_execution_context = () => execution_context_manager
            .Verify(
                _ => _.CurrentFor(
                    Moq.It.IsAny<Execution.ExecutionContext>(),
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<string>()), Moq.Times.Never);
    }
}
