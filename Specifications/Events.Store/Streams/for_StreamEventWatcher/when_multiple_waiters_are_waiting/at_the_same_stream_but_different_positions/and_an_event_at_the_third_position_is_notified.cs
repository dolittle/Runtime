// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_but_different_positions
{
    public class and_an_event_at_the_third_position_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition first_stream_position;
        static StreamPosition second_stream_position;
        static StreamPosition third_stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("14398543-7fa0-44d3-9f45-9fccf2572284");
            stream_id = Guid.Parse("8042de38-727d-4126-b0e6-290413b1898f");
            first_stream_position = 101;
            second_stream_position = 102;
            third_stream_position = 103;
        };

        static Task first_result;
        static Task second_result;
        static Task third_result;
        Because of = () =>
        {
            first_result = event_watcher.WaitForEvent(scope_id, stream_id, first_stream_position, cancellation_token);
            second_result = event_watcher.WaitForEvent(scope_id, stream_id, second_stream_position, cancellation_token);
            third_result = event_watcher.WaitForEvent(scope_id, stream_id, third_stream_position, cancellation_token);
            event_watcher.NotifyForEvent(scope_id, stream_id, third_stream_position);
            Thread.Sleep(100);
        };

        It first_should_be_completed = () => first_result.IsCompleted.ShouldBeTrue();
        It second_should_be_completed = () => second_result.IsCompleted.ShouldBeTrue();
        It third_should_be_completed = () => third_result.IsCompleted.ShouldBeTrue();
    }
}
