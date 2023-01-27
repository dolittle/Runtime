// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection;

public class when_first_read_fails : all_dependencies
{
    static Exception exception;
    static Scenario scenario;

    Establish context = () =>
    {
        exception = new Exception();

        scenario = Scenario.New(_ =>
        {
            _.Receive.Exception(exception).AtTime(10);
        });
    };

    Because of = () => scenario.Simulate(
        request_id,
        server_call_context,
        message_converter.Object,
        metrics,
        logger_factory);

    It should_cancel_the_cancellation_token = () => scenario.ConnectionCancellationToken.IsCancellationRequested.Should().BeTrue();
    It should_have_passed_along_the_read_exception = () => scenario.RuntimeStreamMoveNextException.Should().Be(exception);
    It should_not_schedule_a_ping_callback = () => scenario.ScheduledCallbacks.Should().BeEmpty();
}