// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_reading;

public class and_stream_throws_an_exception : all_dependencies
{
    static Mock<MessageReceived> event_handler;
    static a_message message;
    static Exception exception;
    static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

    Establish context = () =>
    {
        message = new a_message();
        exception = new Exception();

        event_handler = new Mock<MessageReceived>();

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

    Because of = () =>
    {
        wrapped_reader.MessageReceived += event_handler.Object;
        wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
        Catch.Exception(() => wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult());
    };

    It should_call_the_event_handler_twice = () => event_handler.Verify(_ => _(), Times.Once);
}