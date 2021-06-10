// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.when_pings_are_requested_every_12_seconds
{
    public class and_next_message_is_received_after_35_seconds : given.all_dependencies
    {
        Establish context = () =>
        {
            scenario = Scenario.New(_ => {
                _.Receive.Message(first_message_with_12_second_pings).AtTime(20);
                _.Receive.Message(new()).AtTime(55);
            });
        };

        Because of = () => scenario.Simulate(
            request_id,
            server_call_context,
            message_converter.Object,
            metrics,
            logger_factory);

        It should_have_written_pings = () => scenario.WrittenMessageTimes.ShouldContainOnly(20, 32, 44);
        It should_have_set_the_initial_refresh_time = () => scenario.RefreshedTokenTimes.ShouldContainOnly(20, 55);
        It should_not_set_the_cancellation_token = () => scenario.ConnectionCancellationToken.IsCancellationRequested.ShouldBeFalse();
    }
}