// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context
{
    public class and_stream_throws_an_exception_on_the_second_message : all_dependencies
    {
        static a_message message;
        static object message_arguments;
        static ReverseCallArgumentsContext message_arguments_context;
        static Exception exception;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () => 
        {
            message = new();
            message_arguments = new();
            message_arguments_context = new();
            exception = new();

            message_converter
                .Setup(_ => _.GetConnectArguments(message))
                .Returns(message_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(message_arguments))
                .Returns(message_arguments_context);

            wrapped_reader = new(
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

        static ReverseCallArgumentsContext result;
        Because of = () =>
        {
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            result = wrapped_reader.ReverseCallContext.GetAwaiter().GetResult();
            Catch.Exception(() => wrapped_reader.ReverseCallContext.GetAwaiter().GetResult());
        };

        It should_return_the_arguments_context = () => result.ShouldEqual(message_arguments_context);
        It should_call_the_converter_with_the_first_message = () => message_converter.Verify(_ => _.GetConnectArguments(message));
        It should_call_the_converter_with_the_connect_arguments = () => message_converter.Verify(_ => _.GetArgumentsContext(message_arguments));
    }
}
