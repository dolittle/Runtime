// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context
{
    public class and_there_are_two_messages : given.all_dependencies
    {
        static a_message first_message;
        static object first_message_arguments;
        static ReverseCallArgumentsContext first_message_arguments_context;
        static a_message second_message;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () =>
        {
            first_message = new();
            first_message_arguments = new();
            first_message_arguments_context = new();
            second_message = new();

            message_converter
                .Setup(_ => _.GetConnectArguments(first_message))
                .Returns(first_message_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(first_message_arguments))
                .Returns(first_message_arguments_context);

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

            wrapped_reader.ReverseCallContextReceived += reverse_call_context_received.Object;
            wrapped_reader.ReverseCallContextNotReceivedInFirstMessage += reverse_call_context_not_received_in_first_message.Object;
        };

        Because of = () =>
        {
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
        };

        It should_have_invoked_the_received_event_once = () => reverse_call_context_received.Verify(_ => _(first_message_arguments_context), Times.Once);
    }
}
