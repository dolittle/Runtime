// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_receiving_arguments;

public class and_there_are_no_messages_in_client_stream : given.a_dispatcher
{
    static bool result;

    Establish context = () =>
    {
        client_to_runtime_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));
    };

    Because of = () => result = dispatcher.ReceiveArguments(CancellationToken.None).GetAwaiter().GetResult();

    It should_return_false = () => result.ShouldBeFalse();
    It should_not_set_arguments = () => dispatcher.Arguments.ShouldBeNull();
}