// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriterActor.when_pinging;

public class and_writing_fails : given.a_wrapped_stream_writer
{
    static a_message ping_message;
    static Exception exception;

    Establish context = () =>
    {
        exception = new Exception();

        message_converter
            .Setup(_ => _.SetPing(Moq.It.IsAny<a_message>(), Moq.It.IsAny<Ping>()))
            .Callback<a_message, Ping>((message, _) => ping_message = message);

        original_writer
            .Setup(_ => _.WriteAsync(Moq.It.IsAny<a_message>()))
            .Returns(Task.FromException(exception));
    };

    static Exception result;
    Because of = () => result = Catch.Exception(() => writer.MaybeWritePing());

    It should_not_write_the_ping_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(ping_message), Moq.Times.Never);
    It should_not_fail_with_the_original_exception = () => result.ShouldBeNull();
}