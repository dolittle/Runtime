// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context
{
    public class and_there_are_two_messages_with_arguments_context : all_dependencies
    {
        static a_message first_message;
        static object first_message_arguments;
        static ReverseCallArgumentsContext first_message_arguments_context;
        static a_message second_message;
        static object second_message_arguments;
        static ReverseCallArgumentsContext second_message_arguments_context;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () => 
        {
            first_message = new();
            first_message_arguments = new();
            first_message_arguments_context = new();
            second_message = new();
            second_message_arguments = new();
            second_message_arguments_context = new();

            message_converter
                .Setup(_ => _.GetConnectArguments(first_message))
                .Returns(first_message_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(first_message_arguments))
                .Returns(first_message_arguments_context);
            message_converter
                .Setup(_ => _.GetConnectArguments(second_message))
                .Returns(second_message_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(second_message_arguments))
                .Returns(second_message_arguments_context);

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

        static ReverseCallArgumentsContext first_result;
        static ReverseCallArgumentsContext second_result;
        Because of = () =>
        {
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            first_result = wrapped_reader.ReverseCallContext.GetAwaiter().GetResult();
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            second_result = wrapped_reader.ReverseCallContext.GetAwaiter().GetResult();
        };

        It should_return_the_first_arguments_context_the_first_time = () => first_result.ShouldEqual(first_message_arguments_context);
        It should_return_the_first_arguments_context_the_second_time = () => second_result.ShouldEqual(first_message_arguments_context);
    }
}
