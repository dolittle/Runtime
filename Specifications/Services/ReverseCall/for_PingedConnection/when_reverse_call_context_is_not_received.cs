// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection
{
    public class when_reverse_call_context_is_not_received : all_dependencies
    {
        static a_message first_message;
        static Scenario scenario;

        Establish context = () =>
        {
            first_message = new a_message();

            message_converter
                .Setup(_ => _.GetConnectArguments(first_message))
                .Returns(null);

            scenario = Scenario.New(_ =>
            {
                _.Receive.Message(first_message).AtTime(10);
            });
        };

        Because of = () => scenario.Simulate(
            request_id,
            server_call_context,
            message_converter.Object,
            metrics,
            logger_factory);

        It should_cancel_the_cancellation_token = () => scenario.ConnectionCancellationToken.IsCancellationRequested.ShouldBeTrue();
        It should_not_schedule_a_ping_callback = () => scenario.ScheduledCallbacks.ShouldBeEmpty();
    }
}