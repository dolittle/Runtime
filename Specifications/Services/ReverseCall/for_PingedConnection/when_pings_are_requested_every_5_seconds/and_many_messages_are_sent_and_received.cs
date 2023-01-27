// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.when_pings_are_requested_every_5_seconds;

public class and_many_messages_are_sent_and_received : given.all_dependencies
{
    static a_message first_message_to_send;
    static a_message second_message_to_send;

    Establish context = () =>
    {
        first_message_to_send = new a_message();
        second_message_to_send = new a_message();

        scenario = Scenario.New(_ =>
        {
            _.Receive.Message(first_message_with_5_second_pings).AtTime(5);
            _.Receive.Message(new a_message()).AtTime(10);
            _.Send.Message(first_message_to_send).AtTime(12);
            _.Receive.Message(new a_message()).AtTime(14);
            _.Send.Message(second_message_to_send).AtTime(20);
            _.Receive.Message(new a_message()).AtTime(22);
        });
    };

    Because of = () => scenario.Simulate(
        request_id,
        server_call_context,
        message_converter.Object,
        metrics,
        logger_factory);

    It should_have_written_pings_and_messages = () => scenario.WrittenMessageTimes.ShouldContainOnly(5, 10, 12, 15, 20, 20);
    It should_have_refreshed_the_token = () => scenario.RefreshedTokenTimes.ShouldContainOnly(5, 10, 14, 22);
    It should_have_sent_the_two_messages = () => scenario.WrittenMessages.ShouldContain(first_message_to_send, second_message_to_send);
}