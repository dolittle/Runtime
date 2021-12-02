// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Grpc.Core;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriter.given
{
    public class a_wrapped_stream_writer : all_dependencies
    {
        protected static Mock<IAsyncStreamWriter<a_message>> original_writer;
        protected static WrappedAsyncStreamWriter<a_message, a_message, object, object, object, object> wrapped_writer;

        Establish context = () =>
        {
            original_writer = new Mock<IAsyncStreamWriter<a_message>>();

            wrapped_writer = new WrappedAsyncStreamWriter<a_message, a_message, object, object, object, object>(
                request_id,
                original_writer.Object,
                message_converter.Object,
                metrics,
                logger,
                cancellation_token);
        };
    }
}