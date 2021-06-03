// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_reading
{
    public class and_there_are_two_messages : all_dependencies
    {
        static a_message first_message;
        static a_message second_message;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () => 
        {
            first_message = new();
            second_message = new();

            wrapped_reader = new(
                request_id,
                an_async_stream_reader<a_message>
                    .that()
                    .receives(first_message)
                    .receives(second_message)
                    .completes(),
                message_converter.Object,
                metrics,
                logger,
                cancellation_token);
        };

        static bool first_read;
        static a_message first_result;
        static a_message first_result_read_again;
        static bool second_read;
        static a_message second_result;
        static bool third_read;
        Because of = () =>
        {
            first_read = wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            first_result = wrapped_reader.Current;
            first_result_read_again = wrapped_reader.Current;
            second_read = wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            second_result = wrapped_reader.Current;
            third_read = wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
        };

        It should_return_true_for_the_first_read = () => first_read.ShouldBeTrue();
        It should_return_the_first_message_after_the_first_read = () => first_result.ShouldEqual(first_message);
        It should_return_the_first_message_after_the_first_read_when_current_is_read_again = () => first_result_read_again.ShouldEqual(first_message);
        It should_return_true_for_the_second_read = () => second_read.ShouldBeTrue();
        It should_return_the_first_message_after_the_second_result = () => second_result.ShouldEqual(second_message);
        It should_return_true_for_the_third_read = () => third_read.ShouldBeFalse();
    }
}
