// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.when_pings_are_requested_every_5_seconds
{
    public class and_no_further_messages_are_received : given.all_dependencies
    {
        Establish context = () =>
        {
            scenario = Scenario.New(_ =>
            {
                _.Receive.Message(first_message_with_5_second_pings).AtTime(6);
            });
        };

        Because of = () => scenario.Simulate(
            request_id,
            server_call_context,
            message_converter.Object,
            metrics,
            logger_factory);

        It should_schedule_a_ping_callback_every_5_seconds = () => scenario.ScheduledCallbacks.ShouldContainOnly(TimeSpan.FromSeconds(5));
        It should_have_set_the_initial_refresh_time = () => scenario.RefreshedTokenTimes.ShouldContainOnly(6);
    }
}