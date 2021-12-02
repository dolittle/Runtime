// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_subscribing_to_message_received
{
    public class and_stream_throws_an_exception : all_dependencies
    {
        static a_message message;
        static Exception exception;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () =>
        {
            message = new a_message();
            exception = new Exception();

            wrapped_reader = new WrappedAsyncStreamReader<a_message, a_message, object, object, object, object>(
                request_id,
                an_async_stream_reader<a_message>
                    .that()
                    .receives(message)
                    .throws(exception),
                message_converter.Object,
                metrics,
                logger,
                cancellation_token);
        };

        static bool first_read;
        static a_message first_result;
        static Exception second_read;
        Because of = () =>
        {
            first_read = wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            first_result = wrapped_reader.Current;
            second_read = Catch.Exception(() => wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult());
        };

        It should_return_true_for_the_first_read = () => first_read.ShouldBeTrue();
        It should_return_the_first_message_after_the_first_read = () => first_result.ShouldEqual(message);
        It should_throw_exception_for_the_second_read = () => second_read.ShouldEqual(exception);
    }
}
