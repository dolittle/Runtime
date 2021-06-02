// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context
{
    public class and_the_first_message_does_not_contain_context : all_dependencies
    {
        static a_message first_message;
        static object first_message_arguments;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () => 
        {
            first_message = new();
            first_message_arguments = new();

            message_converter
                .Setup(_ => _.GetConnectArguments(first_message))
                .Returns(first_message_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(first_message_arguments))
                .Returns<IConvertReverseCallMessages<a_message, a_message, object, object, object, object>>(null);

            wrapped_reader = new(
                request_id,
                an_async_stream_reader<a_message>
                    .that()
                    .receives(first_message)
                    .completes(),
                message_converter.Object,
                metrics,
                logger,
                cancellation_token);
        };

        static Exception result;
        Because of = () =>
        {
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            result = Catch.Exception(() => wrapped_reader.ReverseCallContext.GetAwaiter().GetResult());
        };

        It should_fail = () => result.ShouldBeOfExactType<ArgumentsContextNotReceivedInFirstMessage>();
    }
}
