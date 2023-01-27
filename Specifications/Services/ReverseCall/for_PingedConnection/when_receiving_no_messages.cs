// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection;

public class when_receiving_no_messages : all_dependencies
{
    static Scenario scenario;

    Establish context = () =>
    {
        scenario = Scenario.New(_ => { });
    };

    Because of = () => scenario.Simulate(
        request_id,
        server_call_context,
        message_converter.Object,
        metrics,
        logger_factory);

    It should_cancel_the_cancellation_token = () => scenario.ConnectionCancellationToken.IsCancellationRequested.Should().BeTrue();
    It should_not_schedule_a_ping_callback = () => scenario.ScheduledCallbacks.Should().BeEmpty();
}