// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_subscribing_to_message_received
{
    public class and_there_are_two_messages : all_dependencies
    {
        static a_message first_message;
        static a_message second_message;
        static Mock<MessageReceived> event_handler;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () => 
        {
            first_message = new();
            second_message = new();

            event_handler = new Mock<MessageReceived>();

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

        Because of = () =>
        {
            wrapped_reader.MessageReceived += event_handler.Object;
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
            wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();
        };

        It should_call_the_event_handler_twice = () => event_handler.Verify(_ => _(), Times.Exactly(2));
    }
}
