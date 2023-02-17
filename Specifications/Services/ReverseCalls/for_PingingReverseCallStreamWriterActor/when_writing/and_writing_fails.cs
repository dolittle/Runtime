// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingingReverseCallStreamWriterActor.when_writing;

public class and_writing_fails : given.a_wrapped_stream_writer
{
    static a_message message;
    static Exception exception;

    Establish context = () =>
    {
        message = new a_message();
        exception = new Exception();

        original_writer
            .Setup(_ => _.WriteAsync(message))
            .Returns(Task.FromException(exception));
    };

    static Exception result;
    Because of = () => result = Catch.Exception(() => writer.WriteAsync(message).GetAwaiter().GetResult());

    It should_write_the_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(message));
    It should_fail_with_the_original_exception = () => result.ShouldEqual(exception);
}