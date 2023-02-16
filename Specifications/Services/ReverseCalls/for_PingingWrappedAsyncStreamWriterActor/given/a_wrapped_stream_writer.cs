// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.given;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingingWrappedAsyncStreamWriterActor.given;

public class a_wrapped_stream_writer : all_dependencies
{
    protected static Mock<IAsyncStreamWriter<a_message>> original_writer;
    protected static WrappedAsyncStreamWriter<a_message, a_message, object, object, object, object> wrapped_writer;
    static ActorSystem actor_system;

    Establish context = () =>
    {
        actor_system = new ActorSystem();
        original_writer = new Mock<IAsyncStreamWriter<a_message>>();
        wrapped_writer = new WrappedAsyncStreamWriter<a_message, a_message, object, object, object, object>(
            true,
            actor_system,
            true,
            request_id,
            original_writer.Object,
            message_converter.Object,
            metrics,
            logger,
            cancellation_token);
    };

    Cleanup cleanup = () =>
    {
        wrapped_writer.Dispose();
        actor_system.DisposeAsync().AsTask().GetAwaiter().GetResult();
    };
}