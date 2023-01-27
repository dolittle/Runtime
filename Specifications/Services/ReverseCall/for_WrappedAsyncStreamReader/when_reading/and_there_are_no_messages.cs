// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.when_reading;

public class and_there_are_no_messages : all_dependencies
{
    static WrappedAsyncStreamReader<a_message, a_message, object, object, object, object> wrapped_reader;

    Establish context = () => wrapped_reader = new WrappedAsyncStreamReader<a_message, a_message, object, object, object, object>(
        request_id,
        an_async_stream_reader<a_message>
            .that()
            .completes(),
        message_converter.Object,
        metrics,
        logger,
        cancellation_token);

    static bool result;
    Because of = () => result = wrapped_reader.MoveNext(cancellation_token).GetAwaiter().GetResult();

    It should_return_false = () => result.Should().BeFalse();
}