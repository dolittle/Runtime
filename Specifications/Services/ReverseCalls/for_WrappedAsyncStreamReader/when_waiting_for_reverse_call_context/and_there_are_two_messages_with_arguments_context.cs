// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context;

public class and_there_are_two_messages_with_arguments_context : given.all_dependencies
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
        first_message = new a_message();
        first_message_arguments = new object();
        first_message_arguments_context = new ReverseCallArgumentsContext();
        second_message = new a_message();
        second_message_arguments = new object();
        second_message_arguments_context = new ReverseCallArgumentsContext();

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

        wrapped_reader = new WrappedAsyncStreamReader<a_message, a_message, object, object, object, object>(
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