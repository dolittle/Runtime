// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitionState
{
    public class when_creating_state
    {
        static StreamPosition position;
        static string reason;
        static DateTimeOffset retry_time;
        static FailingPartitionState state;

        Establish context = () =>
        {
            position = 0;
            reason = "reason";
            retry_time = DateTimeOffset.Now;
        };

        Because of = () =>
            state = new FailingPartitionState
            {
                Position = position,
                Reason = reason,
                RetryTime = retry_time
            };

        It should_have_the_correct_position = () => state.Position.ShouldEqual(position);
        It should_have_the_correct_reason = () => state.Reason.ShouldEqual(reason);
        It should_have_the_correct_retry_time = () => state.RetryTime.ShouldEqual(retry_time);
    }
}