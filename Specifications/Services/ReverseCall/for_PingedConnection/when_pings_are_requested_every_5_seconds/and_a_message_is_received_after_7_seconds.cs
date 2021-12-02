// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.when_pings_are_requested_every_5_seconds
{
    public class and_a_message_is_received_after_7_seconds : given.all_dependencies
    {
        Establish context = () =>
        {
            scenario = Scenario.New(_ =>
            {
                _.Receive.Message(first_message_with_5_second_pings).AtTime(10);
                _.Receive.Message(new a_message()).AtTime(17);
            });
        };

        Because of = () => scenario.Simulate(
            request_id,
            server_call_context,
            message_converter.Object,
            metrics,
            logger_factory);

        It should_schedule_a_ping_callback_every_5_seconds = () => scenario.ScheduledCallbacks.ShouldContainOnly(TimeSpan.FromSeconds(5));
        It should_have_written_a_ping_at_15_seconds = () => scenario.WrittenMessageTimes.ShouldContainOnly(10, 15);
        It should_have_refreshed_the_token_at_17_seconds = () => scenario.RefreshedTokenTimes.ShouldContainOnly(10, 17);
    }
}