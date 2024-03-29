// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_waiting_for_reverse_call_context;

public class and_there_are_no_messages : given.all_dependencies
{
    static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

    Establish context = () =>
    {
        wrapped_reader = new WrappedAsyncStreamReader<a_message, a_message, object, object, object, object>(
            request_id,
            an_async_stream_reader<a_message>
                .that()
                .completes(),
            message_converter.Object,
            metrics,
            logger,
            cancellation_token);

        wrapped_reader.ReverseCallContextReceived += reverse_call_context_received.Object;
        wrapped_reader.ReverseCallContextNotReceivedInFirstMessage += reverse_call_context_not_received_in_first_message.Object;
    };

    Because of = () => wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();

    It should_not_have_invoked_the_received_event = () => reverse_call_context_received.VerifyNoOtherCalls();
    It should_have_invoked_the_not_received_event = () => reverse_call_context_not_received_in_first_message.Verify(_ => _(), Times.Once);
}