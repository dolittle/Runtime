// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.Callbacks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection
{
    public class when_disposing_after_pings_have_started : all_dependencies
    {
        static a_message message_with_call_arguments;
        static object call_arguments;

        static Mock<ICallbackScheduler> callback_scheduler;
        static Mock<IDisposable> scheduled_callback;

        static PingedConnection<a_message, a_message, object, object, object, object> connection;

        Establish context = () =>
        {
            message_with_call_arguments = new();
            call_arguments = new();
            message_converter
                .Setup(_ => _.GetConnectArguments(message_with_call_arguments))
                .Returns(call_arguments);
            message_converter
                .Setup(_ => _.GetArgumentsContext(call_arguments))
                .Returns(new ReverseCallArgumentsContext()
                {
                    PingInterval = Duration.FromTimeSpan(TimeSpan.FromSeconds(20)),
                });

            callback_scheduler = new();
            scheduled_callback = new();
            callback_scheduler
                .Setup(_ => _.ScheduleCallback(Moq.It.IsAny<Action>(), Moq.It.IsAny<TimeSpan>()))
                .Returns(scheduled_callback.Object);

            connection = new(
                request_id,
                an_async_stream_reader<a_message>
                    .that()
                    .receives(message_with_call_arguments),
                Mock.Of<IAsyncStreamWriter<a_message>>(),
                server_call_context,
                message_converter.Object,
                Mock.Of<ICancelTokenIfDeadlineIsMissed>(),
                callback_scheduler.Object,
                metrics,
                logger_factory);
        };

        Because of = () =>
        {
            connection.RuntimeStream.MoveNext(cancellation_token).GetAwaiter().GetResult();
            connection.Dispose();
        };

        It should_dispose_of_the_scheduled_callback = () => scheduled_callback.Verify(_ => _.Dispose(), Times.Once);
    }
}
