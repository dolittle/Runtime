// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriterActor.when_writing;

public class and_writing_fails_once_then_succeeds : given.a_wrapped_stream_writer
{
    static a_message first_message;
    static Exception exception;
    static a_message second_message;

    Establish context = () =>
    {
        first_message = new a_message();
        exception = new Exception();
        second_message = new a_message();

        original_writer
            .Setup(_ => _.WriteAsync(first_message))
            .Returns(Task.FromException(exception));
        original_writer
            .Setup(_ => _.WriteAsync(second_message))
            .Returns(Task.CompletedTask);
    };

    static Exception result;
    Because of = () =>
    {
        result = Catch.Exception(() => wrapped_writer.WriteAsync(first_message).GetAwaiter().GetResult());
        wrapped_writer.WriteAsync(second_message).GetAwaiter().GetResult();
    };

    It should_write_the_first_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(first_message));
    It should_fail_with_the_original_exception = () => result.ShouldEqual(exception);
    It should_write_the_second_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(second_message));
}