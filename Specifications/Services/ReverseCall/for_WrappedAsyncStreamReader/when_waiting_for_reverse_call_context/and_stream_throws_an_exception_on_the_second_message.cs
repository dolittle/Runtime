// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context;

public class and_stream_throws_an_exception_on_the_second_message : given.all_dependencies
{
    static a_message message;
    static object message_arguments;
    static ReverseCallArgumentsContext message_arguments_context;
    static Exception exception;
    static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

    Establish context = () =>
    {
        message = new a_message();
        message_arguments = new object();
        message_arguments_context = new ReverseCallArgumentsContext();
        exception = new Exception();

        message_converter
            .Setup(_ => _.GetConnectArguments(message))
            .Returns(message_arguments);
        message_converter
            .Setup(_ => _.GetArgumentsContext(message_arguments))
            .Returns(message_arguments_context);

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

        wrapped_reader.ReverseCallContextReceived += reverse_call_context_received.Object;
        wrapped_reader.ReverseCallContextNotReceivedInFirstMessage += reverse_call_context_not_received_in_first_message.Object;
    };

    Because of = () =>
    {
        wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
        Catch.Exception(() => wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult());
    };

    It should_have_invoked_the_received_event = () => reverse_call_context_received.Verify(_ => _(message_arguments_context), Times.Once);
    It should_not_have_invoked_the_not_received_event = () => reverse_call_context_not_received_in_first_message.VerifyNoOtherCalls();
    It should_call_the_converter_with_the_first_message = () => message_converter.Verify(_ => _.GetConnectArguments(message));
    It should_call_the_converter_with_the_connect_arguments = () => message_converter.Verify(_ => _.GetArgumentsContext(message_arguments));
}