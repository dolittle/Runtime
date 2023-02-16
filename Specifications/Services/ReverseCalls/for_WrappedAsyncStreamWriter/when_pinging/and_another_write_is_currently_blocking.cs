// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriter.when_pinging;

public class and_another_write_is_currently_blocking : given.a_wrapped_stream_writer
{
    static a_message another_message;

    Establish context = () =>
    {
        another_message = new a_message();

        original_writer
            .Setup(_ => _.WriteAsync(another_message))
            .Returns(Task.Delay(Timeout.Infinite));
    };

    Because of = () =>
    {
        _ = wrapped_writer.WriteAsync(another_message);
        wrapped_writer.MaybeWritePing();
    };

    It should_not_write_the_ping_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(Moq.It.IsAny<a_message>()), Times.Once);
}