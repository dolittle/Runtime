// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_subscribing_to_message_received
{
    public class and_there_are_no_messages : all_dependencies
    {
        static Mock<MessageReceived> event_handler;
        static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

        Establish context = () =>
        {
            event_handler = new Mock<MessageReceived>();

            wrapped_reader = new(
                request_id,
                an_async_stream_reader<a_message>
                    .that()
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
        };

        It should_not_call_the_event_handler = () => event_handler.VerifyNoOtherCalls();
    }
}
